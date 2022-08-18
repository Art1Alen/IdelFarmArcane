using System.Collections;
using UnityEngine;
using DG.Tweening;
public class WheatPack : MonoCashe
{
    #region Public and Private
    public settings Settings;

    private BoxCollider _boxCollider;
    private GameObject _chopParticleEffect;

    [SerializeField] private int _howManyChopsBeforeCutOff;
    [SerializeField] private float _coolDownForNextCut = 1f;

    private float _timeForMowAnimation = 1f;
    private float _timeForWheatGrow = 10f;
    private bool _isCoolDownForTrigger;
    private int _currentChopsBeforeCutOff;
     
    private Transform[] _wheats = new Transform[6];
    private WheatData[] _initData = new WheatData[6];
    #endregion

    #region Monobeh
    private void OnEnable()
    {
        _howManyChopsBeforeCutOff = Settings.HowManyChopsBeforeCutOff;
        _isCoolDownForTrigger = false;
        _currentChopsBeforeCutOff = _howManyChopsBeforeCutOff;
        _boxCollider = GetComponent<BoxCollider>();
        _boxCollider.enabled = true;

        _chopParticleEffect = transform.GetChild(8).gameObject;
        _chopParticleEffect.SetActive(false);

        for (int i = 0; i < _wheats.Length; i++)
        {            
            _wheats[i] = transform.GetChild(i);
            _initData[i] = new WheatData(_wheats[i].localPosition, _wheats[i].rotation, _wheats[i].localScale);            
        }

        //make unique materials
        for (int i = 0; i < _wheats.Length; i++)
        {
            Material newMaterial = Instantiate(transform.GetChild(i).GetComponent<MeshRenderer>().material);
            newMaterial.SetFloat("color_slider", 1f);
            _wheats[i].GetComponent<MeshRenderer>().material = newMaterial;
        }
    }
       
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("scythe") && !_isCoolDownForTrigger)                 
            ChopThisPack();        
    }
    #endregion

    #region ChopThisPack
    private void ChopThisPack()
    {     
        StartCoroutine(coolDownForCutOff());
        StartCoroutine(startChopParticleEffect());

        _currentChopsBeforeCutOff--;

        if (_currentChopsBeforeCutOff == 0)
        {
            _boxCollider.enabled = false;
            StartCoroutine(ChopEffect());            
        }       
    }

    private IEnumerator startChopParticleEffect()
    {
        if (_chopParticleEffect.activeSelf) _chopParticleEffect.SetActive(false);
        _chopParticleEffect.SetActive(true);
        yield return new WaitForSeconds(2);
        _chopParticleEffect.SetActive(false);
    }

    private IEnumerator ChopEffect() 
    {
        Vector3 new_position = new Vector3(transform.position.x, transform.position.y+0.1f, transform.position.z);
        Sequence seq = DOTween.Sequence();
      
        for (int i = 0; i < _wheats.Length; i++)
        {
            seq.Join(_wheats[i].transform.DOMove(new Vector3(transform.position.x + UnityEngine.Random.Range(-0.5f, 0.5f), transform.position.y + 0.1f, transform.position.z + UnityEngine.Random.Range(-0.5f, 0.5f)), _timeForMowAnimation, false));
            seq.Join(_wheats[i].transform.DORotate(new Vector3(80, UnityEngine.Random.Range(-70f, 70f) , 0), _timeForMowAnimation, RotateMode.Fast));            
        }

        seq.AppendInterval(_timeForMowAnimation);

        for (int i = 0; i < _wheats.Length; i++)
        {
            seq.Join(_wheats[i].transform.DOLocalMove(new Vector3(0, 0.5f, 0), _timeForMowAnimation/2, false));
            seq.Join(_wheats[i].transform.DOScale(0, _timeForMowAnimation/2f));
        }

        for (int i = 0; i < _wheats.Length; i++)      
            seq.Join(_wheats[i].transform.DOLocalMove(Vector3.zero, 0.01f, false));            
        
        StartCoroutine(createReadyPack());

        yield return new WaitForSeconds(_timeForMowAnimation*2);

        StartCoroutine(growAgain());        
    }

    private IEnumerator coolDownForCutOff()
    {
        _isCoolDownForTrigger = true;
        yield return new WaitForSeconds(_coolDownForNextCut);
        _isCoolDownForTrigger = false;
    }

    private IEnumerator createReadyPack()
    {
        
        yield return new WaitForSeconds(_timeForMowAnimation);
        GameObject pack = Instantiate(Resources.Load<GameObject>("ReadyWheatStack"), Vector3.zero, Quaternion.identity, transform);
        pack.transform.localPosition = Vector3.zero;
    }

    private IEnumerator growAgain()
    {
        
        yield return new WaitForSeconds(Time.deltaTime);

        Sequence seq1 = DOTween.Sequence();
        
        for (int i = 0; i < _wheats.Length; i++)
        {
            _wheats[i].GetComponent<MeshRenderer>().material.SetFloat("color_slider", 0);
            seq1.Join(_wheats[i].transform.DOScale(Vector3.zero, 0));
            seq1.Join(_wheats[i].transform.DOLocalMove(_initData[i].position, 0, false));
            seq1.Join(_wheats[i].transform.DORotate(_initData[i].rotation.eulerAngles, 0, RotateMode.Fast));
            seq1.Join(_wheats[i].transform.DOScaleX(_initData[i].scale.x, 0));            
        }

        for (int i = 0; i < _wheats.Length; i++)
        {
            seq1.Join(_wheats[i].transform.DOScaleY(_initData[i].scale.y * 0.6f, _timeForWheatGrow * 0.9f));
            seq1.Join(_wheats[i].transform.DOScaleZ(_initData[i].scale.z * 0.6f, _timeForWheatGrow * 0.9f));
            seq1.Join(_wheats[i].GetComponent<MeshRenderer>().material.DOFloat(0.8f, "color_slider", _timeForWheatGrow * 0.9f));
        }

        yield return new WaitForSeconds(_timeForWheatGrow * 0.9f);

        for (int i = 0; i < _wheats.Length; i++)
        {
            seq1.Join(_wheats[i].transform.DOScaleY(_initData[i].scale.y, _timeForWheatGrow * 0.1f));
            seq1.Join(_wheats[i].transform.DOScaleZ(_initData[i].scale.z, _timeForWheatGrow * 0.1f));
            seq1.Join(_wheats[i].GetComponent<MeshRenderer>().material.DOFloat(1, "color_slider", 0.1f));
        }

        yield return new WaitForSeconds(_timeForWheatGrow * 0.1f);

        _currentChopsBeforeCutOff = _howManyChopsBeforeCutOff;
        _boxCollider.enabled = true;
    }
    #endregion

    #region Struct WheatData
    public struct WheatData
    {
        public Vector3 position, scale;
        public Quaternion rotation;

        public WheatData(Vector3 _pos, Quaternion _rot, Vector3 _scale)
        {
            position = _pos;
            rotation = _rot;
            scale = _scale;
        }
    }
    #endregion
}
