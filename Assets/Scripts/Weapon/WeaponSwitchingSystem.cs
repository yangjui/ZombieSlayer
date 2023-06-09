using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwitchingSystem : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private WeaponBase[] weapons;

    [HideInInspector]
    public PlayerHUD playerHUD;
    private WeaponBase currentWeapon;
    private WeaponBase previousWeapon;
    private WeaponAssaultRifle weaponAssaultRifle;
    private int index = 0;
    private float timer = 0f;
    private float currentTime = 0f;

    private void Awake()
    {
        weaponAssaultRifle = GetComponentInChildren<WeaponAssaultRifle>();
    }

    public void Init()
    {
        playerHUD.SetupAllWeapons(weapons);

        for (int i = 0; i < weapons.Length; ++i)
        {
            if (weapons[i].gameObject != null)
            {
                weapons[i].gameObject.SetActive(false);
            }
        }
        SwitchingWeapon(WeaponType.레이저라이플);
        SwitchingWeapon(WeaponType.수류탄);
        SwitchingWeapon(WeaponType.중력자탄);
        SwitchingWeapon(WeaponType.돌격소총);
    }

    private void Update()
    {
        if (Time.timeScale == 0) return;
        UpdateSwitch();
    }

    private void UpdateSwitch()
    {
        if (/*!Input.anyKeyDown || */weaponAssaultRifle.isAimMode) return;

        int inputIndex = 0;
        // 입력된 값이 1~4 사이의 숫자라면 inputIndex에 입력값이 저장되고
        // 입력값에 맞는 무기가 호출된다.
        if (int.TryParse(Input.inputString, out inputIndex) && (inputIndex > 0 && inputIndex < 5))
        {
            SwitchingWeapon((WeaponType)(inputIndex - 1));
            index = inputIndex - 1;
        }

        timer = Time.time;
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && timer - currentTime > 0.2f)
        {
            index++;
            if (index > 3) index = 0;
            SwitchingWeapon((WeaponType)(index));
            currentTime = timer;
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0 && timer - currentTime > 0.2f)
        {
            index--;
            if (index < 0)
                index = 3;
            SwitchingWeapon((WeaponType)(index));
            currentTime = timer;
        }
    }

    private void SwitchingWeapon(WeaponType _weaponType)
    {
        if (weapons[(int)_weaponType] == null) return;

        if (currentWeapon != null)
        {
            previousWeapon = currentWeapon;
        }
        // 무기교체
        currentWeapon = weapons[(int)_weaponType];

        // 현재 무기와 쓰던 무기가 같다면 return
        if (currentWeapon == previousWeapon) return;

        // 현재 무기 정보 전달
        playerController.SwitchingWeapon(currentWeapon);
        playerHUD.SwitchingWeapon(currentWeapon);

        if (previousWeapon != null)
        {
            // 이전에 사용하던 무기 비활성화
            previousWeapon.gameObject.SetActive(false);
        }

        // 현재 사용하는 무기 활성화
        currentWeapon.gameObject.SetActive(true);
    }

    // 해당 타입 하나의 무기 탄창만 증가시킴
    public void IncreaseMagazine(WeaponType _weaponType, int _magazine)
    {
        if (weapons[(int)_weaponType] != null)
        {
            weapons[(int)_weaponType].IncreaseMagazine(_magazine);
        }
    }

    // 모든 무기의 무기 탄창 수를 증가시킴
    public void IncreseMagazine(int _magazine)
    {
        for (int i = 0; i < weapons.Length; ++i)
        {
            if (weapons[i] != null)
            {
                weapons[i].IncreaseMagazine(_magazine);
            }
        }
    }
}
