using UnityEngine;
public class PoolOfGameObjects : MonoCashe
{
    #region Public and Private
    private GameObject[] _objects;
    #endregion

    #region For
    public PoolOfGameObjects(int capacity, GameObject _object, Transform _transform)
    {
        _objects = new GameObject[capacity];
        for (int i = 0; i < capacity; i++)
        {
            _objects[i] = Instantiate(_object, _transform);
            _objects[i].SetActive(false);
        }
    }

    public GameObject GetFreeObject()
    {
        for (int i = 0; i < _objects.Length; i++)
        {
            if (!_objects[i].activeSelf)
            {
                return _objects[i];
            }
        }
        return _objects[0];
    }
    #endregion
}
