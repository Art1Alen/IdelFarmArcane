using UnityEngine;
public class WheatSpawn : MonoCashe
{
    #region Public and Private
    public float rowX = 5, rowY = 5;
    public Vector3 wheatPackPlace;
    #endregion

    #region Monobeh
    private void Start()
    {
        int i = 0;
        for (float x = wheatPackPlace.x; x < (wheatPackPlace.x + rowX); x++)
        {
            for (float z = wheatPackPlace.z; z >(wheatPackPlace.z - rowY); z--)
            {
                GameObject pack = Instantiate(Resources.Load<GameObject>("WheatPack"), new Vector3(x, 0, z), Quaternion.identity, transform);
                pack.name = i.ToString();
                
                i++;
            }
        }
    }
    #endregion
}
