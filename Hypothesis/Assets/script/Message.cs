
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text.RegularExpressions;
 
public class Message : MonoBehaviour {
 
    //　メッセージUI
    private Text messageText;
    //　表示するメッセージ
    [SerializeField]
    [TextArea(1, 20)]
    private string allMessage = "今回はRPGでよく使われるメッセージ表示機能を作りたいと思います。\n"
            + "メッセージが表示されるスピードの調節も可能であり、改行にも対応します。\n"
            + "改善の余地がかなりありますが、               最低限の機能は備えていると思われます。\n"
            + "ぜひ活用してみてください。\n<>"
            + "あああああああああああああああああああああああああああああああああああああああああああああああああああああああああああああああああああ"
            + "あああああああああああああああああああああああああああああああああああああああああああああああああああああああああああああああああああ"
            + "あああああああああああああああああああああああああああああああああああああああああああああああああああああああああああああああああああ"
            + "あああああああああああああああああああああああああああああああああああああああああああああああああああああああああああああああああああ"
            + "あああああああああああああああああああああああああああああああああああああああああああああああああああああああああああああああああああ<>"
            + "あああああああああああああああああああああああああああああああああああああああああああああああああああああああああああああああああああ"
            + "あああああああああああああああああああああああああああああああああああああああああああああああああああああああああああああああああああ"
            + "あああああああああああああああああああああああああああああああああああああああああああああああああああああああああああああああああああ"
            + "あああああああああああああああああああああああああああああああああああああああああああああああああああああああああああああああああああ<>"
            + "あああああああああああああああああああああああああああああああああああああああああああああああああああああああああああああああああああ"
            + "あああああああああああああああああああああああああああああああああああああああああああああああああああああああああああああああああああ"
            + "あああああああああああああああああああああああああああああああああああああああああああああああああああああああああああああああああああ"
            + "あああああああああああああああああああああああああああああああああああああああああああああああああああああああああああああああああああ";
    //　使用する分割文字列
    [SerializeField]
    private string splitString = "<>";
    //　分割したメッセージ
    private string[] splitMessage;
    //　分割したメッセージの何番目か
    private int messageNum;
    //　テキストスピード
    [SerializeField]
    private float textSpeed = 0.05f;
    //　経過時間
    private float elapsedTime = 0f;
    //　今見ている文字番号
    private int nowTextNum = 0;
    //　マウスクリックを促すアイコン
    private Image clickIcon;
    //　クリックアイコンの点滅秒数
    [SerializeField]
    private float clickFlashTime = 0.2f;
    //　1回分のメッセージを表示したかどうか
    private bool isOneMessage = false;
    //　メッセージをすべて表示したかどうか
    public bool isEndMessage = false;

    private GameObject player;
    PlayerController playerScript;
 
    void Start() {
        clickIcon = transform.Find("Panel/Image").GetComponent<Image>();
        clickIcon.enabled = false;
        player = GameObject.Find("robo");
        playerScript = player.GetComponent<PlayerController>();
        messageText = GetComponentInChildren<Text>();
        messageText.text = "";
        transform.GetChild(0).gameObject.SetActive(false);
        SetMessage(allMessage);
    }
 
    void Update() {
        //　メッセージが終わっているか、メッセージがない場合はこれ以降何もしない
        if (isEndMessage || allMessage == null) {
            return;
        }
 
        //　1回に表示するメッセージを表示していない	
        if (!isOneMessage) {
            //　テキスト表示時間を経過したらメッセージを追加
            if (elapsedTime >= textSpeed) {
                messageText.text += splitMessage[messageNum][nowTextNum];
 
                nowTextNum++;
                elapsedTime = 0f;
 
                //　メッセージを全部表示、または行数が最大数表示された
                if (nowTextNum >= splitMessage[messageNum].Length) {
                    isOneMessage = true;
                }
            }
            elapsedTime += Time.deltaTime;
 
            //　メッセージ表示中にマウスの左ボタンを押したら一括表示
            if (Input.GetMouseButtonDown(0)) {
                //　ここまでに表示しているテキストに残りのメッセージを足す
                messageText.text += splitMessage[messageNum].Substring(nowTextNum);
                isOneMessage = true;
            }
        //　1回に表示するメッセージを表示した
        } else {
 
            elapsedTime += Time.deltaTime;
 
            //　クリックアイコンを点滅する時間を超えた時、反転させる
            if (elapsedTime >= clickFlashTime) {
                clickIcon.enabled = !clickIcon.enabled;
                elapsedTime = 0f;
            }
 
            //　マウスクリックされたら次の文字表示処理
            if (Input.GetMouseButtonDown(0)) {
                nowTextNum = 0;
                messageNum++;
                messageText.text = "";
                clickIcon.enabled = false;
                elapsedTime = 0f;
                isOneMessage = false;
 
                //　メッセージが全部表示されていたらゲームオブジェクト自体の削除
                if (messageNum >= splitMessage.Length) {
                    isEndMessage = true;
                    transform.GetChild(0).gameObject.SetActive(false);
                }
            }
        }
    }
    //　新しいメッセージを設定
    void SetMessage(string message) {
        this.allMessage = message;
        //　分割文字列で一回に表示するメッセージを分割する
        splitMessage = Regex.Split(allMessage, @"\s*" + splitString + @"\s*", RegexOptions.IgnorePatternWhitespace);
        nowTextNum = 0;
        messageNum = 0;
        messageText.text = "";
        isOneMessage = false;
        isEndMessage = false;
    }
    //　他のスクリプトから新しいメッセージを設定しUIをアクティブにする
    public void SetMessagePanel(string message) {
        SetMessage(message);
        transform.GetChild(0).gameObject.SetActive(true);
    }
}
