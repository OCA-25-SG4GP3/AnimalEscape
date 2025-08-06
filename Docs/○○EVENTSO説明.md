# 📣 ○○EventSO 使用方法ガイド

このドキュメントでは、`○○EventSO`（ScriptableObject を利用したイベント通知システム）の基本的な使い方について説明します。

---

## 🔧 1. 概要

`○○EventSO` は、イベント（UnityAction）を ScriptableObject 経由で送信・登録できる仕組みです。

- **発信者**と**受信者**を疎結合にできる
- 複数のリスナーを登録可能
- InvokeEvent() を呼ぶことで、登録しているすべてのリスナーに通知が送られます。

---

## 📦 2. ScriptableObject の作成
1. Unity の Project ウィンドウで右クリック
2. Create > Events > ○○ Event を選択
3. 任意の名前でアセットを保存（例：OnButtonEvent）

---

## 🧑‍💻 3. 発信側スクリプト（イベントを送る）
```csharp
public class Button : MonoBehaviour
{
    [SerializeField] private VoidEventSO _onButtonEvent;

    void OnTriggerEnter(Collider other)
    {
        // イベントを発信
        _onButtonEvent.InvokeEvent();
    }
}
```

---

## 🎧 4. 受信側スクリプト（イベントを受け取る）
```csharp
public class Door : MonoBehaviour
{
    [SerializeField] private VoidEventSO _onButtonEvent;

    private void OnEnable()
    {
        _onButtonEvent.OnEventInvoked += DoorOpen;
    }

    private void OnDisable()
    {
        _onButtonEvent.OnEventInvoked -= DoorOpen;
    }

    private void DoorOpen()
    {
        Debug.Log("ドアを開く");
    }
}
```

---

## 📝 5. 注意点
OnEnable/OnDisable でイベント登録を行うことで、登録解除によるメモリリークやバグを防げます。

UnityAction を使っているため、ラムダ式でも登録可能ですが、登録解除のためには明示的な関数を使うことを推奨します。

---

## ✅ 6. 使用例（イベントの活用）
シーン遷移時の通知

UI の更新トリガー

オブジェクトのスポーン通知

プレイヤーの死亡・リスタート通知など

---

## 💬 おわりに
○○EventSO を使用することで、コードの依存関係を最小限にし、柔軟かつ再利用性の高い設計が可能になります。
ゲーム全体のアーキテクチャ改善に役立ててください。

---
