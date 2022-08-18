using UnityEngine;
public class CurrentPlayerState : MonoCashe
{
    #region Public and Private
    private Animator _currentAnimator;
    #endregion

    #region StateAnimator
    public CurrentPlayerState(Animator _animator) => _currentAnimator = _animator;
    
    public void SetState(states newState)
    {
        switch ((int)newState)
        {
            case 0:
                Idle();
                break;

            case 1:
                Run();
                break;

            case 2:
                Mow();
                break;
        }
    }
    private void Idle()
    {
        if (_currentAnimator.GetCurrentAnimatorStateInfo(0).IsTag("idle"))
            return;

        resetMow();
        _currentAnimator.StopPlayback();
        _currentAnimator.Play("idle");
    }

    private void Run()
    {
        if (_currentAnimator.GetCurrentAnimatorStateInfo(0).IsTag("run"))
            return;

        resetMow();
        _currentAnimator.StopPlayback();
        _currentAnimator.Play("run");
    }

    private void Mow()
    {
        if (!_currentAnimator.GetCurrentAnimatorStateInfo(0).IsTag("run") && !_currentAnimator.GetCurrentAnimatorStateInfo(0).IsTag("idle"))
            _currentAnimator.SetLayerWeight(0, 1f);

        if (_currentAnimator.GetCurrentAnimatorStateInfo(1).IsTag("mow"))
            return;

        _currentAnimator.SetLayerWeight(1, 1f);
        _currentAnimator.StopPlayback();
        _currentAnimator.Play("mow");
    }
   
    private void resetMow() 
    {
        if (!_currentAnimator.GetCurrentAnimatorStateInfo(1).IsTag("mow"))
            _currentAnimator.SetLayerWeight(1, 0);
    }
    #endregion

    #region Enum
    public enum states
    {
        idle,
        run,
        mow
    }
    #endregion
}

