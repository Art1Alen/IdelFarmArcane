using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoCashe
{
    public settings Settings;

    private bool _isCoinImageBusy;

    private Camera _currentCamera;
    private Image _wheatAmount, _coinImage;
    private PoolOfGameObjects _getCoinForEffect;
    private TextMeshProUGUI _moneyTextMesh, _currentPack, _maxPack;

    [SerializeField] private int _money = 0;
    [SerializeField] private int _howMuchForOneStack;
    [SerializeField] private int _maxBagCapacity;

    // Start is called before the first frame update
    private void Start()
    {
        _howMuchForOneStack = Settings.howMuchForOneStack;
        _maxBagCapacity = Settings.MaxBagCapacity;
        _moneyTextMesh = GameObject.Find("Money").GetComponent<TextMeshProUGUI>();
        _currentPack = GameObject.Find("current").GetComponent<TextMeshProUGUI>();
        _maxPack = GameObject.Find("max").GetComponent<TextMeshProUGUI>();
        _maxPack.text = _maxBagCapacity.ToString();
        _currentPack.text = "0";
        _moneyTextMesh.text = "0";

        _getCoinForEffect = new PoolOfGameObjects(50, Resources.Load<GameObject>("Coin"), transform);

        _coinImage = GameObject.Find("CoinImage").GetComponent<Image>();
        _wheatAmount = GameObject.Find("wheatAmount").GetComponent<Image>();
        _wheatAmount.fillAmount = 0;

        _currentCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        
    }



    public void SetWheatAmount(int currentAmount)
    {
        _wheatAmount.fillAmount = (float)currentAmount / (float)_maxBagCapacity;
        _currentPack.text = currentAmount.ToString();
    }

    public void SetEffectOfCoins() => StartCoroutine(coinsFlyToCoinIcon());


    private IEnumerator AddCoins(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            _money += 1;
            _moneyTextMesh.text = _money.ToString();
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    private IEnumerator coinsFlyToCoinIcon()
    {        
        Transform Coin = _getCoinForEffect.GetFreeObject().transform;
        Coin.localScale = Vector3.zero;
        Coin.gameObject.SetActive(true);
        Coin.position = _currentCamera.WorldToScreenPoint(GameObject.Find("PointOfBarn").transform.position);

        Sequence seq = DOTween.Sequence();

        seq.Join(Coin.DOScale(1, 1));
        seq.Join(Coin.DOMove(new Vector3(_coinImage.transform.position.x-5, _coinImage.transform.position.y-25, _coinImage.transform.position.z), 1));

        yield return new WaitForSeconds(1f);
        Coin.gameObject.SetActive(false);
        StartCoroutine(AddCoins(_howMuchForOneStack));

        if (!_isCoinImageBusy)
        {
            _isCoinImageBusy = true;
            _coinImage.transform.GetChild(0).transform.DOShakeScale(0.3f);
            yield return new WaitForSeconds(0.3f);
            _isCoinImageBusy = false;
        }
    }
}
