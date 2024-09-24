using System.Collections;
using UnityEngine;

public class DistanceToPlayer : MonoBehaviour
{
    private StripManager _stripManager;
    private CivilianCar _civilianCar;
    private Transform _playerPosition;

    private float _backTeleportDistance;
    private float _frontTeleportDistance;

    private bool _lastCar = false;
    private bool _firstCar = false;
    private bool _carSrashed = false;
    public bool LastCar { set { _lastCar = value; } }
    public bool FirstCar { set { _firstCar = value; } }
    public Transform PlayerPosition { get { return _playerPosition; } }

    public void Init(StripManager stripManager, Transform player, float backDistance, float frontDistance)
    {
        _civilianCar = GetComponent<CivilianCar>();
        _playerPosition = player;
        _stripManager = stripManager;
        _backTeleportDistance = backDistance;
        _frontTeleportDistance = frontDistance;
    }

    public void SetCrashedCar()
    {
        _carSrashed = true;
        StartCoroutine(CarCrashedCoroutine());
    }

    public void SetLastCar()
    {
        _lastCar = true;
        StartCoroutine(CheckPlayerInFrontCoroutine());
    }

    public void SetFirstCar()
    {
        _firstCar = true;
        StartCoroutine(CheckPlayerInBackCoroutine());
    }

    private IEnumerator CarCrashedCoroutine()
    {
        while (_carSrashed) 
        {
            yield return new WaitForFixedUpdate();
            Vector3 playerPosition = _playerPosition.position;
            Vector3 carPosition = transform.position;
            float distance = Vector3.Distance(playerPosition, carPosition);
            if(distance > _backTeleportDistance)
            {
                _carSrashed = false;
                _lastCar = false; 
                _firstCar = false;
                _stripManager.RespawnCarBackToFront(_civilianCar);
                yield break;
            }
        }
    }

    private IEnumerator CheckPlayerInFrontCoroutine()
    {
        while(_lastCar) 
        {
            yield return new WaitForFixedUpdate();
            float playerPosition = _playerPosition.position.z;
            float carPosition = transform.position.z;
            float distance = playerPosition - carPosition;
            if (distance > _backTeleportDistance)
            {
                _lastCar = false;
                _stripManager.RespawnCarBackToFront(_civilianCar);
                yield break;
            }
        }
    }

    private IEnumerator CheckPlayerInBackCoroutine()
    {
        while(_firstCar)
        {
            yield return new WaitForFixedUpdate();
            float playerPosition = _playerPosition.position.z;
            float carPosition = transform.position.z;
            float distance = carPosition - playerPosition;
            if (distance > _frontTeleportDistance)
            {
                _firstCar = false;
                _stripManager.RespawnCarFrontToBack(_civilianCar);
                yield break;
            }
        }
    }
    public void StopCheckPlayerInBackCoroutine()
    {
        StopCoroutine(CheckPlayerInBackCoroutine());
    }

    public void StopCheckPlayerInFrontCoroutine()
    {
        StopCoroutine(CheckPlayerInFrontCoroutine());
    }

    private void OnDisable()
    {
        StopCoroutine(CheckPlayerInBackCoroutine());
        StopCoroutine(CheckPlayerInFrontCoroutine());
        StopCoroutine(CarCrashedCoroutine());
    }

    private void OnDestroy()
    {
        StopCoroutine(CheckPlayerInBackCoroutine());
        StopCoroutine(CarCrashedCoroutine());
        StopCoroutine(CheckPlayerInFrontCoroutine());

    }
    private void OnApplicationQuit()
    {
        StopCoroutine(CheckPlayerInBackCoroutine());
        StopCoroutine(CarCrashedCoroutine());
        StopCoroutine(CheckPlayerInFrontCoroutine());
    }
}
