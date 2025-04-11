using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    // 卡片的状态
    public enum Card_state
    {
        被遮挡,
        显示,
        隐藏
    }
    public Card_state state;
    // 卡片和遮挡卡片的深度
    public Vector2[] depth = new Vector2[3];
    // 用于移动卡片的变量
    private Transform CardTransform;
    // 用于移动遮挡卡片的变量
    private Transform CardTransform2;
    
    void Awake(){
        CardTransform = transform;
        CardTransform2 = transform.GetChild(0).transform;
    }

    // 设置卡片的状态
    public void SetState(Card_state state){
        this.state = state;
        // 改变卡片的位置
        CardTransform.localPosition = new Vector3(CardTransform.localPosition.x, CardTransform.localPosition.y, depth[(int)state].x);
        // 改变遮挡卡片的位置
        CardTransform2.localPosition = new Vector3(CardTransform2.localPosition.x, CardTransform2.localPosition.y, depth[(int)state].y);  
    }

}
