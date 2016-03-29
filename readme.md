CocoAction.unity
====

cocos2d-x style action for Unity3d.
<br>

Example
----
```c#
RepeatForever.Create(
  MoveBy.Create(2.0f, 200, 100));
```

Additional Features
----
__RepeatForever_with_Sequence__
```c#
RepeatForever.Create(
  MoveBy.Create(1.0f, 200,0),
  DelayTime.Create(1.0f),
  MoveBy.Create(1.0f, -200,0),
  DelayTime.Create(1.0f));
```
__RepeatUntil__
```c#
RepeatUntil.Create(
  MoveBy.Create(1.0f, 200,0),
  () => {
    // true -> keep playing,
    // falase -> stop
    return isDone;
  });
```

ToDo
-----
* 자체적인 액션 처리 (현재 NGUI TWEEN)
* EASSSSSEEEEING
* pause/resume