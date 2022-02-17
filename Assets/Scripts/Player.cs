using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField] private Transform gun;
    [SerializeField] private GameObject bulletPrefab, indicationBall;
    private Camera _camera;
    private Renderer _currentColor;
    private GameManager _gameManager;

    [SerializeField] private float sensivity, shotForce, coldDownTime;
    private float _rotationSpeed;
    private bool _reload;
    private int _currentColorIndex;

    private void Start()
    {
        _camera = Camera.main;
        _gameManager = FindObjectOfType<GameManager>();
        _currentColor = indicationBall.GetComponent<Renderer>();
        _currentColorIndex = Random.Range(0, _gameManager.ballsColors.Length);
        _currentColor.material = _gameManager.ballsColors[_currentColorIndex];
        _rotationSpeed = 1 + sensivity;
    }

    private void Update()
    {
        var mousePosition = GetMousePosition();

        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(mousePosition),
            Time.deltaTime * _rotationSpeed);

        if (Input.GetMouseButtonDown(0) && !_reload && !_gameManager.pause)
            StartCoroutine(Shot());
    }

    private IEnumerator Shot()
    {
        _reload = true;
        indicationBall.SetActive(false);

        var bullet = Instantiate(bulletPrefab, gun.position + gun.forward, Quaternion.identity);

        bullet.GetComponent<Bullet>().colorIndex = _currentColorIndex;
        bullet.GetComponent<Renderer>().material = _currentColor.material;
        bullet.GetComponent<Rigidbody>().AddForce(gun.forward * shotForce);

        _currentColorIndex = Random.Range(0, _gameManager.ballsColors.Length);
        _currentColor.material = _gameManager.ballsColors[_currentColorIndex];

        yield return new WaitForSeconds(coldDownTime);

        _reload = false;
        indicationBall.SetActive(true);
    }


    private Vector3 GetMousePosition()
    {
        var mousePosition = new Vector3();
        Plane plane = new Plane(Vector3.up, 0);

        float distance;
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        if (plane.Raycast(ray, out distance))
        {
            mousePosition = ray.GetPoint(distance);
        }

        return mousePosition;
    }

    public void SetSensivity()
    {
        var value = _gameManager.sensitivitySlider.value;
        _rotationSpeed = 1 + sensivity * value / .7f;
    }
}