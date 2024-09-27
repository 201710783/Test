using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ScrollBar : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    [SerializeField] int characterNumer;      //(임시)현재 가지고 있는 캐릭터의 수를 표시
    [SerializeField] GameObject prefab;       //캐릭터 칸의 역할을 하는 오브젝트
    [SerializeField] GameObject content;      //자식으로 가진 캐릭터 칸을 배열해주는 오브젝트

    [SerializeField] Scrollbar scrollbar;     //Scrollbar를 움직이기 위한 Scrollbar.Value를 가진 오브젝트
    [SerializeField] ScrollRect scrollrect;   //ScrollRect를 가진 오브젝트
    [SerializeField] Button frontButton;      //FrontButton 오브젝트
    [SerializeField] Button backButton;       //BackButton 오브젝트


    int slotCount;                  //현재 가지고 있는 캐릭터 수를 int로 저장하기 위한 변수

    IEnumerator timer;              //코루틴을 종료시키기 위한 변수
    float dragTime;                 //드래그 시간을 기록하기 위한 변수
    int dragPower;                  //드래그 시의 이동 칸 수
    Vector2 beginPoint;             //드래그 시작 위치를 확인하기 위한 변수
    Vector2 endPoint;               //드래그 종료 위치를 확인하기 위한 변수
    bool scroll;                    //스크롤 이동 중 드래그를 방지하기 위한 변수

    WaitForSeconds waitTime = new WaitForSeconds(0.01f); //Scroll 코루틴의 칸 이동에 사용되는 딜레이 시간
    float target;                   //Scroll 코루틴의 이동 목적지를 나타내는 변수


    const int minSlotCount = 5;         //캐릭터 칸의 최소 갯수는 5개
    const float minScrollValue = 0f;    //스크롤바의 최소 값(가장 앞의 칸 표시)
    const float maxScrollValue = 1f;    //스크롤바의 최대 값(가장 뒤의 칸 표시)

    const float shortDragTime = 0.2f;
    const float dragTimeWeight = 10f;   //드래그 시간에 따른 dragPower 증가량
    const float dragPointWeight = 0.002f;   //드래그 길이에 따흔 dragPower 증가량
    const float scrollSpeed = 0.2f;     //스크롤이 이동하는 속도

    const int buttonPower = 5;          //버튼을 눌렀을 떼 이동하는 칸 수
    const int minDragPower = 1;         //드래그 시 최소 이동 칸 수

    void Start()
    {
        scrollrect.horizontal = false; 
        
        if(characterNumer < minSlotCount) //캐릭터 칸의 수는 최소 5칸
        {
            characterNumer = minSlotCount;
        }

        if (prefab != null) //Resources.Load를 이용하여 캐릭터 수에 맞춰 슬롯을 생성
        {
            for(int i = 0; i < characterNumer; i++)
            {
                Instantiate(prefab, content.transform);
            }            
        }
        
        slotCount = characterNumer;             //(임시)캐릭터 정보를 int가 아닌 list등으로 가져와야 할 경우에는 해당 내용을 수정 
        InteractableButton(scrollbar.value);    //스크롤 가능한 내용을 초기화

        frontButton.onClick.AddListener(OnClickFrontButton); //버튼과 함수를 연결
        backButton.onClick.AddListener(OnClickBackButton);
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    public void OnBeginDrag(PointerEventData eventData) //드래그 시작 시 시작위치와 시간확인
    {
        if(!scroll)   
        {
            dragTime = 0;
            beginPoint = eventData.position;
            
            timer = Timer();
            StartCoroutine(timer);
        }
    }

    
    public void OnEndDrag(PointerEventData eventData) //드래그 종료 시 시작 위치와 시간을 토대로 스크롤을 움직인다.
    {
        if(!scroll)
        {
            if(timer != null)
            {
                StopCoroutine(timer);
            }

            endPoint = eventData.position;

            if(dragTime < shortDragTime) //드래그를 짧게할 경우 1칸 이동
            {
                dragPower = minDragPower;
            }

            else //드래그를 길게할 경우 드래그 시간과 크기에 비례하여 이동
            {
                dragPower = (int)(dragTime * dragTimeWeight * Mathf.Abs(beginPoint.x - endPoint.x) * dragPointWeight);
            }

            if(beginPoint.x < endPoint.x) //드래그 방향에 따라 스크롤바 이동 방향 구분
            {
                StartCoroutine(BackScroll(dragPower));
            }

            else
            {
                StartCoroutine(FrontScroll(dragPower));
            }
        }
    }

    
    IEnumerator Timer() //드래그 시간을 확인하기 위한 코루틴
    {
        while(true)
        {
            yield return null;
            dragTime += Time.deltaTime;
        }
    }

    public void OnClickFrontButton()
    {   
        StartCoroutine(FrontScroll(buttonPower));
    }

    IEnumerator FrontScroll(int count) //스크롤의 Value를 증가시키는 코루틴
    {
        frontButton.interactable = false; //스크롤바 이동 중 다시 이동하는 것을 막기위한 처리
        backButton.interactable = false; 
        scroll = true;

        target = scrollbar.value + (maxScrollValue / (slotCount - minSlotCount)) * count;
        if(target >= maxScrollValue)
        {
            target = maxScrollValue;
        }

        while(scrollbar.value < target) //반복문을 통하여 이동
        {
            yield return waitTime;
            scrollbar.value += (maxScrollValue / (slotCount - minSlotCount)) * scrollSpeed;
        }

        scrollbar.value = target; //스크롤 위치가 약간 어긋나는 것을 방지
        yield return null;

        InteractableButton(scrollbar.value); //스크롤 위치를 바탕으로 버튼을 활성화 하는 함수
    }

    public void OnClickBackButton()
    {
        StartCoroutine(BackScroll(buttonPower));
    }

    IEnumerator BackScroll(int count) //스크롤의 Value를 감소시키는 코루틴
    {
        frontButton.interactable = false; //스크롤바 이동 중 다시 이동하는 것을 막기위한 처리
        backButton.interactable = false;
        scroll = true;

        target = scrollbar.value - (maxScrollValue / (slotCount - minSlotCount)) * count;
        if(target <= minScrollValue)
        {
            target = minScrollValue;
        }

        while(scrollbar.value > target) //반복문을 통하여 이동
        {
            yield return waitTime;
            scrollbar.value -= (maxScrollValue / (slotCount - minSlotCount)) * scrollSpeed;
        }

        scrollbar.value = target; //스크롤의 위치가 약간 어긋나는 것을 방지
        yield return null;

        InteractableButton(scrollbar.value); //스크롤 위치를 바탕으로 버튼을 활성화 하는 함수
    }

    //현재 스크롤바 위치에 따른 버튼의 비활성화 함수
    public void InteractableButton(float value)
    {
        frontButton.interactable = value < (1f / (slotCount - minSlotCount)) * (slotCount - 5.5); //마지막 칸과 그 전 칸 사이를 기준으로 front버튼의 비활성화를 구분
        backButton.interactable =  value > (1f / (slotCount - minSlotCount)) * (0.5);

        scroll = false; //드래그는 활성화
    }
    
}
