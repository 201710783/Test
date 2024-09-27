using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ScrollBar : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    public int characterNumer;      //(임시) 현재 가지고 있는 캐릭터의 수를 표시
    public GameObject prefab;        //(임시)캐릭터 칸의 역할을 하는 오브젝트, Resources.Load<GameObject>를 이용하여 가져오는 것도 가능
    public GameObject content;      //자식으로 가진 캐릭터 칸을 배열해주는 오브젝트, GetComponentInChildren을 이용하여 가져오는 것도 가능
    int slotCount;                  //현재 가지고 있는 캐릭터 수를 int로 저장하기 위한 변수

    public Scrollbar scrollbar;     //GetComponentInChildren을 이용하여 가져오는 것도 가능
    public ScrollRect scrollrect;   //this.GetComponent로 가져오는 것도 가능
    public Button frontButton;      //버튼을 public으로 구현 public을 사용하지 않고 부모 오브젝트인 canvas를 거쳐
    public Button backButton;       //GetComponentsInChildren등을 이용하여 가져오는 것도 가능.
                                    //단, 모든 GetComponent등의 무거운 작업은 Start에서 1번만 실행

    IEnumerator timer;              //코루틴을 종료시키기 위한 변수
    float dragTime;                 //드래그 시간을 기록하기 위한 변수
    Vector2 beginPoint;             //드래그 시작 위치를 확인하기 위한 변수
    Vector2 endPoint;               //드래그 종료 위치를 확인하기 위한 변수
    bool scroll;                    //스크롤 이동 중 드래그를 방지하기 위한 변수

    void Start()
    {
        scrollrect.horizontal = false; 
        
        if(characterNumer < 5) //캐릭터 칸의 수는 최소 5칸
        {
            characterNumer = 5;
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
            int power;

            if(dragTime < 0.2) //드래그를 짧게할 경우 1칸 이동
            {
                power = 1;
            }

            else //드래그를 길게할 경우 드래그 시간과 크기에 비례하여 이동
            {
                power = (int)(dragTime * 10 * Mathf.Abs(beginPoint.x - endPoint.x) / 500);
            }

            //Debug.Log(dragTime);
            //Debug.Log(Mathf.Abs(beginPoint.x - endPoint.x));
            //Debug.Log(power);

            if(beginPoint.x < endPoint.x) //드래그 방향에 따라 스크롤바 이동 방향 구분
            {
                StartCoroutine(BackScroll(power));
            }

            else
            {
                StartCoroutine(FrontScroll(power));
            }
        }
    }

    
    IEnumerator Timer() //드래그 시간을 확인하기 위한 코루틴
    {
        while(true)
        {
            yield return null;
            dragTime += Time.deltaTime;
            //Debug.Log(dragTime);
        }
    }

    public void OnClickFrontButton()
    {   
        StartCoroutine(FrontScroll(5));
    }

    IEnumerator FrontScroll(int count) //스크롤의 Value를 증가시키는 코루틴
    {
        frontButton.interactable = false; //스크롤바 이동 중 다시 이동하는 것을 막기위한 처리
        backButton.interactable = false; 
        scroll = true;

        float value = scrollbar.value; //이동거리 계산
        float target = value + (1f / (slotCount - 5)) * count;
        if(target >= 1)
        {
            target = 1;
        }

        while(scrollbar.value < target) //반복문을 통하여 이동
        {
            yield return new WaitForSeconds(0.01f);
            scrollbar.value += (1f / (slotCount - 5)) / 5;
        }

        scrollbar.value = target; //스크롤 위치가 약간 어긋나는 것을 방지
        yield return null;

        InteractableButton(scrollbar.value); //스크롤 위치를 바탕으로 버튼을 활성화 하는 함수
    }

    public void OnClickBackButton()
    {
        StartCoroutine(BackScroll(5));
    }

    IEnumerator BackScroll(int count) //스크롤의 Value를 감소시키는 코루틴
    {
        frontButton.interactable = false; //스크롤바 이동 중 다시 이동하는 것을 막기위한 처리
        backButton.interactable = false;
        scroll = true;

        float value = scrollbar.value; //이동거리 계산
        float target = value - (1f / (slotCount - 5)) * count;
        if(target <= 0)
        {
            target = 0;
        }

        while(scrollbar.value > target) //반복문을 통하여 이동
        {
            yield return new WaitForSeconds(0.01f);
            scrollbar.value -= (1f / (slotCount - 5)) / 5;
        }

        scrollbar.value = target; //스크롤의 위치가 약간 어긋나는 것을 방지
        yield return null;

        InteractableButton(scrollbar.value); //스크롤 위치를 바탕으로 버튼을 활성화 하는 함수
    }

    //현재 스크롤바 위치에 따른 버튼의 비활성화 함수
    public void InteractableButton(float value)
    {
        //마지막 칸과 그 전 칸 사이를 기준으로 front버튼의 비활성화를 구분
        if(value > (1f / (slotCount - 5)) * (slotCount - 5.5))
        {
            frontButton.interactable = false;
        }
        
        else
        {
            frontButton.interactable = true;
        }

        //첫 칸과 그 다음 칸 사이를 기준으로 back버튼의 비활성화를 구분
        if(value < (1f / (slotCount - 5)) * 0.5)
        {
            backButton.interactable = false;
        }

        else
        {
            backButton.interactable = true;
        }

        scroll = false;
    }
    
}
