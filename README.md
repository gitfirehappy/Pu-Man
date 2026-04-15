# Pu-Man - 类幸存者肉鸽游戏

基于 Unity 开发的 2D 幸存者类 Roguelike 游戏，玩家在无尽波次中生存战斗，通过选择 Buff 强化角色。

## 技术栈

- **引擎**：Unity 2022.3+
- **语言**：C#
- **插件**：DOTween、Addressables、New Input System

## 核心技术亮点

| 技术 | 应用场景 |
|------|----------|
| **泊松盘采样** | 敌人组/组内敌人生成，保证均匀分布 |
| **Boids 群聚算法** | 敌人 AI 聚集、分离、对齐行为 |
| **SO 架构** | Buff / 角色 / 敌人数据配置化管理 |
| **事件队列系统** | 全局状态切换解耦，优先级调度 |
| **对象池** | 分类自动回收，保障 60fps 稳定运行 |

## 项目结构

```
Assets/Scripts/
├── Enemy/              # 敌人系统（生成器、AI、组件）
│   └── AlgorithmHelper/# 泊松采样 + Boids 算法
├── Player/             # 玩家系统（移动、射击、Buff）
├── SO/                 # ScriptableObject 数据配置
├── EventLines/         # 事件总线 + 队列管理
├── PoolManager/        # 对象池管理
├── UI/                 # UI 框架（MVC 模式）
├── LevelSystem/        # 关卡状态机 + 波次管理
└── Volume/             # 音频管理系统
```

## 游戏特性

- 多角色可选，各有独特技能体系
- 波次递进难度，Boss 战机制
- Buff 升级系统（普通/稀有/史诗/传说四阶）
- 暂停/继续/设置 完整 UI 流程

## 运行方式

1. 使用 Unity Hub 打开项目
2. 进入 `Assets/Scenes/` 加载主场景
3. 运行游戏

## License

MIT
