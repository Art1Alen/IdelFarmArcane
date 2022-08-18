using UnityEngine;
public class GlobalUpdate : MonoBehaviour
{
    #region Monobeh
    private void Update()
    {
        for (int i = 0; i < MonoCashe.allUpdate.Count; i++) MonoCashe.allUpdate[i].Tick();
    }
    #endregion
}
