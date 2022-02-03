using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private Transform gun;
    [SerializeField] private GameObject bulletPrefab, indicationBall;
    private Camera _camera;
    private Renderer _currentColor;
    private GameManager _gameManager;

    [SerializeField] private float rotationSpeed, shotForce, coldDownTime;
    private bool _reload;
    private int _currentColorIndex;

    private void Start()
    {
        _camera = Camera.main;
        _gameManager = FindObjectOfType<GameManager>();
        _currentColor = indicationBall.GetComponent<Renderer>();
        _currentColorIndex = Random.Range(0, _gameManager.ballsColors.Length);
        _currentColor.material = _gameManager.ballsColors[_currentColorIndex];
    }

    private void Update()
    {
        var mousePosition = GetMousePosition();

        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(mousePosition),
            Time.deltaTime * rotationSpeed);

        if (Input.GetMouseButtonDown(0) && !_reload)
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
}