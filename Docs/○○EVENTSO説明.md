# 📣 ○○EventSO 使用方法ガイド

このドキュメントでは、`○○EventSO`（ScriptableObject を利用したイベント通知システム）の基本的な使い方について説明します。

---

## 🔧 1. 概要

`○○EventSO` は、イベント（UnityAction）を ScriptableObject 経由で送信・登録できる仕組みです。

- **発信者**と**受信者**を疎結合にできる
- 複数のリスナーを登録可能
- InvokeEvent() を呼ぶことで、登録しているすべてのリスナーに通知が送られます。

---

## 📌 2. 不健全な依存関係の例（ScriptableObject未使用）
```csharp
public class Player : MonoBehaviour
{
    public Door door; // 他オブジェクトに直結している

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Key"))
        {
            door.Open(); // 直接ドアの関数を呼び出している
        }
    }
}
```
🔻 問題点
- Player クラスが Door クラスに直接依存している。
- オブジェクト同士が相互に強く結びついてしまい、再利用性・テスト性・柔軟性が下がる。
- シーン内のオブジェクトの結合が密になり、管理が困難になる。

---

## 📦 3. ScriptableObject の作成
1. Unity の Project ウィンドウで右クリック
2. Create > Events > ○○ Event を選択
3. 任意の名前でアセットを保存（例：OnButtonEvent）

---

## 🧑‍💻 4. 発信側スクリプト（イベントを送る）
```csharp
public class Player : MonoBehaviour
{
    [SerializeField] private VoidEventSO _onKeyCollectedEvent;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Key"))
        {
            // イベントを発信するだけ
            _onKeyCollectedEvent.InvokeEvent();
        }
    }
}
```

---

## 🎧 5. 受信側スクリプト（イベントを受け取る）
```csharp
public class Door : MonoBehaviour
{
    [SerializeField] private VoidEventSO _onKeyCollectedEvent;

    private void OnEnable()
    {
        _onKeyCollectedEvent.OnEventInvoked += Open;
    }

    private void OnDisable()
    {
        _onKeyCollectedEvent.OnEventInvoked -= Open;
    }

    private void Open()
    {
        Debug.Log("ドアが開く");
    }
}
```

---

## 🧠 6. ゲームオブジェクトの依存関係例
以下のように、ゲーム内のボタンとドアが疎結合でつながるシナリオを考えてみましょう。

```css
[Player] ---> [VoidEventSO: OnKeyCollectedEvent] ---> [Door]
```
- Player：プレイヤーが入ると _onKeyCollectedEvent.InvokeEvent() を呼び出す
- VoidEventSO: OnKeyCollectedEvent：イベントデータの ScriptableObject（シングルトンのように共有される）
- Door：_onKeyCollectedEvent.OnEventInvoked を受信して、イベントを受け取ったらドアが開く

このようにすると、Player は Door を直接知らずに済みます。
アセット参照による依存関係が ScriptableObject 経由になるため、Prefab の再利用やテストが容易になります。

---

## 📝 7. 注意点
OnEnable/OnDisable でイベント登録を行うことで、登録解除によるメモリリークやバグを防げます。

UnityAction を使っているため、ラムダ式でも登録可能ですが、登録解除のためには明示的な関数を使うことを推奨します。

---

## ✅ 8. 使用例（イベントの活用）
シーン遷移時の通知

UI の更新トリガー

オブジェクトのスポーン通知

プレイヤーの死亡・リスタート通知など

---

## 💬 おわりに
○○EventSO を使用することで、コードの依存関係を最小限にし、柔軟かつ再利用性の高い設計が可能になります。
ゲーム全体のアーキテクチャ改善に役立ててください。

---
