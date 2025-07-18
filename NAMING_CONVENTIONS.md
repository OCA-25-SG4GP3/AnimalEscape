# Unity プロジェクト命名規則

このドキュメントは、本プロジェクトで使用するファイル、アセット、コードの命名規則を定義します。チームでの開発をスムーズに進めるために、明確で一貫性のある命名を徹底しましょう。

---

## 📁 1. プロジェクトフォルダ構成

Assets/  
├── Scripts/ -- C#スクリプトを全部このフォルダに入れます  
│├── Player/ -- 中に細かく分類する  
│├── Enemy/  
│└── UI/  
├── Prefabs/ -- Unityのプレハブ、できれば中にも分類する  
├── Scenes/ -- Unityのシーン、他の人のシーンを影響しないため、自分の開発シーンを作ってください  
├── Models/ -- 3Dモデルやアニメション  
├── UI/ -- UI要素  
└── Audio/ -- 音素材  
　├── BGM/  
　└── SFX/  

---

## 🗂 2. ファイル＆アセットの命名規則

### ✅ 基本ルール
- **パスカルケース（PascalCase）** を基本とする（例：`PlayerController.cs`）
- 半角スペースや特殊文字は使用しない（必要なら `_` を使う）
- **短くても意味が通る名前**を心がける
- Unityのオブジェクト/クラス名とファイル名は基本的に一致させる

---

### 📜 C#スクリプト

| 用途            | 例                       |
|-----------------|--------------------------|
| MonoBehaviour   | `PlayerController.cs`   |
| マネージャー     | `GameManager.cs`       |
| UI スクリプト    | `MainMenuUI.cs`         |

---

### 🎨 アセット＆プレハブ

| タイプ            | 命名例                   |
|------------------|--------------------------|
| プレハブ         | `EnemyGoblin.prefab`      |
| UI プレハブ      | `UI_HealthBar.prefab`     |
| モデル           | `Goblin_Idle.fbx`         |
| アニメーション    | `Player_Walk.anim`        |
| 効果音           | `SFX_Jump.wav`            |
| BGM              | `BGM_Battle.mp3`         |
| ScriptableObject | `LevelData.asset`        |

---

### 🎬 シーン

| タイプ  | 命名例                        |
|---------|------------------------------|
| メニュー | `MainMenu.unity`             |
| ステージ | `Level01.unity`              |
| テスト   | `Test_PlayerMovement.unity`  |

---

## 💻 3. コード命名規則（C#）

### ✅ 基本ルール
- クラス名、メソッド名、publicなフィールドには **パスカルケース** を使用
- ローカル変数や private フィールドには **キャメルケース**（先頭小文字）を使用（例：`rotateSpeed`）
- `private` な変数は `_` で始める（例：`_speed`）
- できれば、クラス名は **名詞**、メソッド名は **動詞** で命名
- 略語は一般的なもの（`UI`, `ID`など）以外は避ける
- 定数（const）は全部大文字を使用（例：`MAX_SPEED`）
- 1ファイルにすべてのクラス詰め込むを避ける
- Unity のライフサイクル関数（Start, Updateなど）はクラス内の最上部に記述
- 複雑なロジックにはコメントを付ける

---

### 🧱 クラス & 構造体（例）

```csharp
public class PlayerController { ... }
public struct EnemyState { ... }
```

### ⚙️ 関数

```csharp
public void TakeDamage(int amount) { ... }
private void MoveForward() { ... }
```

### 📦 変数

```csharp
[SerializeField] private float _speed;
private int _health;
public string PlayerName;
public bool IsGrounded { get; private set; }
```

### 🧾 定数

```csharp
private const float GRAVITY = 9.81f;
```

### ✅ 列挙型

```csharp
public enum EnemyType {
    Goblin,
    Orc,
    Boss
}
```