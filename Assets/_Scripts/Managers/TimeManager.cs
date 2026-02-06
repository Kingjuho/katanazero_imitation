using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class TimeManager : MonoBehaviour
{
    [Header("시간 설정")]
    [SerializeField] private float _slowdownFactor = 0.5f;  // 느려지는 정도(0.3 = 0.3배 느려짐)
    [SerializeField] private float _slowdownLength = 2f;    // 정상으로 복귀하는 속도

    [Header("비주얼 설정")]
    [SerializeField] private Volume _globalVolume;          // 글로벌 볼륨
    [SerializeField] private float _maxAberration = 1.0f;   // 색수차 최대 강도
    [SerializeField] private float _maxVignette = 0.35f;    // 색수차 최대 강도

    private ChromaticAberration _chromaticAberration;       // 색수차
    private Vignette _vignette;                             // 비네팅

    private bool _isSlowingDown = false;                    // 현재 슬로우 상태인지 여부

    private void Start()
    {
        VolumeProfile profile = _globalVolume.profile;

        if (!profile.TryGet(out _chromaticAberration))
            Debug.LogError("Chromatic Aberration 효과를 추가해주세요.");

        if (!profile.TryGet(out _vignette))
            Debug.LogError("Vignette 효과를 추가해주세요.");
    }

    private void Update()
    {
        // 우클릭 중이면 슬로우모션, 떼면 복구
        if (Input.GetMouseButton(1))
        {
            if (!_isSlowingDown) OnSlowMotionStart();
            else SlowMotion();
        }
        else
        {
            if (_isSlowingDown) OnSlowMotionEnd();
            else RestoreTime();
        }

        UpdateVisuals();
    }

    /** 슬로우 모션 시작 **/
    private void OnSlowMotionStart()
    {
        _isSlowingDown = true;
        SlowMotion();
    }

    /** 슬로우 모션 종료 **/
    private void OnSlowMotionEnd()
    {
        _isSlowingDown = false;
        RestoreTime();
    }

    /** 슬로우 모션 실행 **/
    private void SlowMotion()
    {
        // 시간 조작
        Time.timeScale = _slowdownFactor;
        // 물리 연산 주기 동기화
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
    }

    /** 슬로우 모션 해제 **/
    private void RestoreTime()
    {
        // 1.0까지 천천히 회복
        Time.timeScale += (1f / _slowdownLength) * Time.unscaledDeltaTime;

        // 1.0이 넘지 않도록 조정
        Time.timeScale = Mathf.Clamp(Time.timeScale, 0f, 1f);

        // 물리 연산 주기 동기화
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
    }

    /** 슬로우 모션 시 카메라 비주얼 **/
    private void UpdateVisuals()
    {
        // timeScale이 작을 수록 t값이 커짐
        float t = 1f - Time.timeScale;

        // 색수차
        if (_chromaticAberration != null)
            _chromaticAberration.intensity.value = t * _maxAberration;

        // 비네팅
        if (_vignette != null)
            _vignette.intensity.value = t * _maxVignette;
    }
}
