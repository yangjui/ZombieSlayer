using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public delegate void ChangeAimModeDelegate(bool _aimMode);
    private ChangeAimModeDelegate changeAimModeCallback = null;

    public delegate void PlayerIsDeadDelegate();
    private PlayerIsDeadDelegate playerIsDeadCallback = null;

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform startPos;
    [SerializeField] private PlayerHUD playerHUD;
    
    private GameObject go;

    public void Init()
    {
        go = Instantiate(playerPrefab, startPos.position, Quaternion.identity);
        go.GetComponent<WeaponSwitchingSystem>().playerHUD = playerHUD;
        go.GetComponent<WeaponSwitchingSystem>().Init();
        go.GetComponentInChildren<WeaponAssaultRifle>().OnChangeAimModeDelegate(OnChangeAimMode);
        go.GetComponent<PlayerController>().SetUnderAttackDelegate(StartBloodScreenCoroutine, OnPlayerIsDead);
        StartCoroutine("LaserRifleDelegateCoroutine");
        StartCoroutine("GrenadeDelegateCoroutine");
        StartCoroutine("GravityGrenadeDelegateCoroutine");
    }

    private IEnumerator LaserRifleDelegateCoroutine()
    {
        GameObject laserRifle = null;
        while (laserRifle == null)
        {
            laserRifle = GameObject.Find("LaserRifle");
            yield return null;
        }
        go.GetComponentInChildren<WeaponLaserRifle>().OnChangeChargeModeDelegate(OnChangeChargeMode);
        StopCoroutine("LaserRifleDelegateCoroutine");
    }

    private IEnumerator GrenadeDelegateCoroutine()
    {
        GameObject Grenade = null;
        while (Grenade == null)
        {
            Grenade = GameObject.Find("Hand Grenade");
            yield return null;
        }
        go.GetComponentInChildren<WeaponGrenade>().SetTrajectotyDelegate(OnOffChangeTrajectory);
        StopCoroutine("GrenadeDelegateCoroutine");
    }

    private IEnumerator GravityGrenadeDelegateCoroutine()
    {
        GameObject GravityGrenade = null;
        while (GravityGrenade == null)
        {
            GravityGrenade = GameObject.Find("Gravity Grenade");
            yield return null;
        }
        go.GetComponentInChildren<WeaponGravityGrenade>().SetTrajectotyDelegate(OnOffChangeTrajectory);
        StopCoroutine("GravityGrenadeDelegateCoroutine");
    }

    public Transform PlayerPosition()
    {
        return go.GetComponent<PlayerController>().PlayerPosition();
    }

    private void OnChangeAimMode(bool _aimMode)
    {
        changeAimModeCallback?.Invoke(_aimMode);
        go.GetComponent<PlayerRotate>().ChangeAimMode(_aimMode);
        go.GetComponent<PlayerController>().ChangeAimMode(_aimMode);
    }

    private void OnChangeChargeMode(bool _chargeMode)
    {
        go.GetComponent<PlayerController>().ChangeChargeMode(_chargeMode);
    }

    private void OnOffChangeTrajectory(bool _trajectory)
    {
        go.GetComponentInChildren<GrenadeTrajectory>().OnOffTrajectory(_trajectory);
    }

    private void StartBloodScreenCoroutine()
    {
        playerHUD.StartBloodScreenCoroutine();
    }

    private void OnPlayerIsDead()
    {
        go.GetComponent<PlayerController>().PlayerIsDead();
        playerIsDeadCallback?.Invoke();
    }

    public void OnClear()
    {
        go.GetComponent<PlayerController>().OnClear();
    }

    public void OnChangeAimModeDelegate(ChangeAimModeDelegate _changeAimModeCallback)
    {
        changeAimModeCallback = _changeAimModeCallback;
    }

    public void OnPlayerIsDeadDelegate(PlayerIsDeadDelegate _playerIsDeadCallback)
    {
        playerIsDeadCallback = _playerIsDeadCallback;
    }
}
