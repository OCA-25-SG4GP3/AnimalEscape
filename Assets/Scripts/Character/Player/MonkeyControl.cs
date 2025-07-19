using UnityEngine;

public class MonkeyControl : PlayerBase
{
    // Update is called once per frame
    void Update()
    {
        float x = _inputSystem.Monkey.Move.ReadValue<Vector2>().x;
        float z = _inputSystem.Monkey.Move.ReadValue<Vector2>().y;

        _movement = new Vector3(x, 0, z).normalized;

        if (_movement != Vector3.zero)
        {
            var rotation = Quaternion.LookRotation(_movement);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, _rotateSpeed);
        }
    }
}
