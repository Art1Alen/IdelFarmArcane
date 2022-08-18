using System.Collections;
using UnityEngine;

public class PlayerManager : MonoCashe
{
    #region Public and Private

    public settings Settings;
    public Joystick CurrentJoystick;

    [SerializeField] private float _playerSpeed;
    [SerializeField] private int _collectedWheatPackAmount;
    [SerializeField] private int _maxBagCapacity;
    [SerializeField] private float _timeBeforeActivateScytheMow = 0.8f;
    [SerializeField] private float _timeForMowActiveWork = 0.15f;
    [SerializeField] private Rigidbody _playerRigidbody;

    private CurrentPlayerState _playerState;
    private UIManager _UI;
    private SphereCollider _scytheCollider;
    private GameObject _scythe;
    private BackPackForWheat _bag;  

    #endregion

    #region Monobeh
    private void Start()
    {
        Screen.SetResolution(720, 1280, true);
        Camera.main.aspect = 9f / 16f;
        Application.targetFrameRate = 60;

        _scythe = GameObject.Find("ScytheV1");
        _scythe.SetActive(false);
        _scytheCollider = _scythe.GetComponent<SphereCollider>();
        _scytheCollider.isTrigger = false;
        _playerRigidbody = GetComponent<Rigidbody>();

        _playerState = new CurrentPlayerState(GetComponent<Animator>());

        _playerSpeed = Settings.playerSpeed;

        _maxBagCapacity = Settings.MaxBagCapacity;
        _bag = GameObject.Find("BackPack").GetComponent<BackPackForWheat>();
        _UI = GameObject.Find("Canvas").GetComponent<UIManager>();
        gameObject.CompareTag("wheat");
        gameObject.CompareTag("wheatpack");
        gameObject.CompareTag("base");
    }
    public override void OnTick() => Move();
    #endregion

    #region MovePlayer
    private void Move()
    {
        _playerRigidbody.velocity = new Vector3(CurrentJoystick.Horizontal * _playerSpeed, _playerRigidbody.velocity.x, CurrentJoystick.Vertical * _playerSpeed);
        if (CurrentJoystick.Horizontal != 0 || CurrentJoystick.Vertical != 0)
        {
            transform.rotation = Quaternion.LookRotation(_playerRigidbody.velocity);
            Run();
        }
        else
            Idle();      
    }
    #endregion

    #region Trigger
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("wheat") && _collectedWheatPackAmount < _maxBagCapacity)      
            Mow();       
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("wheat") && _collectedWheatPackAmount < _maxBagCapacity)                
            Mow();
       
        if (other.CompareTag("wheatpack") && _bag.isReadyToTakeStack())       
            ObtainReadyWheatPack(other.gameObject);
        
        if (other.CompareTag("base") && _bag.isReadyToGiveAwayStack())                  
            giveAwayReadyWheatPack(other.transform);        
    }
    #endregion

    #region Animation
    public void ActivateScythe() => StartCoroutine(ActiveWorkOfScythe());
    public void DeactivateScythe() => _scythe.SetActive(false);    
    private void giveAwayReadyWheatPack(Transform placeToGiveAwayWheat)
    {
        _collectedWheatPackAmount = _bag.GiveAwayReadyWheatPack(placeToGiveAwayWheat);
        _UI.SetWheatAmount(_collectedWheatPackAmount);
    }
    #endregion

    #region Wheat
    private void ObtainReadyWheatPack(GameObject pack)
    {
        _collectedWheatPackAmount = _bag.TakeReadyWheatPack(pack.GetComponent<ReadyWheatPack>());
        _UI.SetWheatAmount(_collectedWheatPackAmount);
    }
    private IEnumerator ActiveWorkOfScythe()
    {
        _scythe.SetActive(true);
        yield return new WaitForSeconds(_timeBeforeActivateScytheMow);

        _scytheCollider.isTrigger = true;
        yield return new WaitForSeconds(_timeForMowActiveWork);

        _scytheCollider.isTrigger = false;
    }

    #endregion

    #region State
    private void Run() => _playerState.SetState(CurrentPlayerState.states.run);
    private void Idle() => _playerState.SetState(CurrentPlayerState.states.idle);
    private void Mow() => _playerState.SetState(CurrentPlayerState.states.mow);
    #endregion
   
}