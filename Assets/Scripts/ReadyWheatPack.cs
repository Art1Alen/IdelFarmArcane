using UnityEngine;
using DG.Tweening;
public class ReadyWheatPack : MonoCashe
{
    #region Public and Private
    [SerializeField] private float _timeForReadyPackToBeActive = 0.4f;

    private BoxCollider _boxCollider;

    private float _angle;
   
    private bool _isRotating = true;
    #endregion

    #region Monobeh
    private void OnEnable()
    {
        _boxCollider = GetComponent<BoxCollider>();
        _boxCollider.enabled = false;
        Invoke("SetReadyToInteract", _timeForReadyPackToBeActive);
        transform.DOShakeScale(_timeForReadyPackToBeActive);
    }   
    public override void OnTick() => TestUpdate();
    private void TestUpdate()
    {
        if (_isRotating)
        {
            _angle += Time.deltaTime * 40f;
            transform.rotation = Quaternion.AngleAxis(_angle, Vector3.up);
        }                
    }
    #endregion

    #region TakePlayer
    public void TakenByPlayer()
    {
        _isRotating = false;
        _boxCollider.enabled = false;        
    }
   
    private void SetReadyToInteract() => _boxCollider.enabled = true;
    #endregion
}
