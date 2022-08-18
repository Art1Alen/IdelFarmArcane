using System.Collections.Generic;
using UnityEngine;
public class MonoCashe : MonoBehaviour
{
    #region Public and Private
    public static List<MonoCashe> allUpdate = new List<MonoCashe>();
    #endregion

    #region Monobeh
    private void OnEnable() => allUpdate.Add(this);
    private void OnDisable() => allUpdate.Remove(this);
    private void OnDestroy() => allUpdate.Remove(this);
    public void Tick() => OnTick();

    public virtual void OnTick() { }

    #endregion
}