using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class WeaponLaserRifle : WeaponBase
{
    [Header("# SpawnPoints")]
    [SerializeField] private Transform chargeEffectPoint;
    [SerializeField] private Transform laserEffectPoint;

    [Header("# Prefab")]
    [SerializeField] private GameObject chargeEffectPrefab = null;
    [SerializeField] private GameObject laserEffectPrefab = null;
    [SerializeField] private GameObject waveEffectPrefab = null;

    [Header("# LaserSetting")]
    [SerializeField] private float chargingTime = 2f;
    [SerializeField] private float chargingSize = 0.1f;

    private Vector3 attackDirection;
    private Vector3 attackRotation;
    private GameObject chargeEffect;
    private Camera mainCamera;

    private bool isTakeOut = false;
    private bool isCharging = false;
    private float currentChargingTime = 0f;

    private void Awake()
    {
        base.SetUp();
        // 첫 탄 수 최대탄수로 설정
        weaponSetting.currentAmmo = weaponSetting.maxAmmo;
        // 첫 탄창 수 최대탄창수로 설정
        weaponSetting.currentMagazine = weaponSetting.maxMagazine;
        mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        // 무기가 활성화될 때 해당 무기의 탄 수를 갱신.
        onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);
        onMagazineEvent.Invoke(weaponSetting.currentMagazine);

        ResetVariables();
    }

    private void OnDisable()
    {
        CancelLaser();
    }

    private IEnumerator OnReload()
    {
        isReload = true;

        anim.OnReload();
        
        while (true)
        {
            if (!isReload)
            {
                if (WeaponName != 0)
                {
                    weaponSetting.currentMagazine--;
                    onMagazineEvent.Invoke(weaponSetting.currentMagazine);
                }

                weaponSetting.currentAmmo = weaponSetting.maxAmmo;
                onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);

                yield break;
            }
            yield return null;
        }
    }

    private void ResetVariables() // 총 바꿀때마다 기본상태로 리셋
    {
        isReload = false;
        isAttack = false;
    } 

    private void TwoStepRaycast()
    {
        Ray ray;
        RaycastHit hit;
        Vector3 targetPoint = Vector3.zero;

        ray = mainCamera.ViewportPointToRay(Vector2.one * 0.5f);
        if (Physics.Raycast(ray, out hit, weaponSetting.attackDistance))
        {
            // 레이가 맞는다면 타겟포인트는 맞은대상
            targetPoint = hit.point;
        }
        else
        {
            // 맞은 대상이 없다면 해당방향의 최대사거리위치
            targetPoint = ray.origin + ray.direction * weaponSetting.attackDistance;
        }
        Debug.DrawRay(ray.origin, ray.direction * weaponSetting.attackDistance, Color.red);

        attackDirection = targetPoint - chargeEffectPoint.position;
    }

    private IEnumerator ChargingLaserCoroutine()
    {
        Debug.Log("StartCharging!");
        isCharging = true;
        currentChargingTime = 0f;
        chargeEffect = Instantiate(chargeEffectPrefab, chargeEffectPoint.position, chargeEffectPoint.rotation);
        chargeEffect.transform.SetParent(chargeEffectPoint);
        chargeEffect.transform.localScale *= 0;
        while (isCharging)
        {
            //Debug.Log("ChargingTime : " + currentChargingTime);
            currentChargingTime += Time.deltaTime;
            if (currentChargingTime < chargingTime)
                chargeEffect.transform.localScale += Vector3.one * currentChargingTime * Time.deltaTime * chargingSize;
            yield return null;
        }
    }

    private void ShotLaser()
    {
        TwoStepRaycast();
        isCharging = false;
        StopCoroutine("ChargingLaserCoroutine");
        if (currentChargingTime >= chargingTime)
        {
            Destroy(chargeEffect);
            Instantiate(laserEffectPrefab, laserEffectPoint.position, Quaternion.LookRotation(attackDirection,Vector3.up) * Quaternion.Euler(0f, 180f, 0f));
            GameObject wave = Instantiate(waveEffectPrefab, laserEffectPoint.position, transform.rotation);
            Destroy(wave, 2f);

            // 공격 시 currentAmmo 1 감소
            weaponSetting.currentAmmo--;

            // 탄 수 UI 업데이트
            onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);

            currentChargingTime = 0f;
        }
        else Destroy(chargeEffect);
    }

    private void CancelLaser()
    {
        isCharging = false;
        currentChargingTime = 0f;
        StopCoroutine("ChargingLaserCoroutine");
        Destroy(chargeEffect);
    }

    public override void StartWeaponAction(int type = 0)
    {
        // 준비상태거나 장전중이면 무기 액션 할 수 없게
        if (isReload || isTakeOut) return;
        Debug.Log(type);
        if (type == 0)
        {
            if (!weaponSetting.isAutomaticAttack)
            {
                OnAttack();
            }
        }
    }

    public override void StopWeaponAction(int type = 0)
    {
        ShotLaser();
    }

    public override void StartReload()
    {
        if (isReload || isTakeOut || weaponSetting.currentMagazine <= 0) return; // 장전 중 재장전 불가능
        CancelLaser(); // 무기 사용중일 수 있으니 무기사용 멈춰줌

        StartCoroutine("OnReload");
    }

    public void OnAttack()
    {
        if (Time.time - lastAttackTime > weaponSetting.attackRate) // 공격주기가 되어야 공격할 수 있다.
        {
            if (anim.MoveSpeed > 0.5f || isTakeOut) return; // 준비상태거나 달리기중일땐 공격불가

            //공격주기를 알 수 있도록 현재 시간 저장
            lastAttackTime = Time.time;

            // 총알이 없다면 return
            if (weaponSetting.currentAmmo <= 0) return;

            isAttack = true;

            StartCoroutine("ChargingLaserCoroutine");

            // 무기 애니메이션 재생
            anim.Play("Fire", -1, 0);
        }
    }


    /// 애니메이션함수, 콜백

    public void IsStopAttack()
    {
        isAttack = false;
    }

    public void IsTakeOutStart()
    {
        SoundManager.instance.Play2DSFX("take_out_weapon", transform.position);
        isTakeOut = true;
    }

    public void IsTakeOutOver()
    {
        isTakeOut = false;
    }

    public void IsReloadStart()
    {
        SoundManager.instance.Play2DSFX("assault_rifle_reload_out", transform.position);
    }

    public void IsReloadOver()
    {
        isReload = false;
    }
}