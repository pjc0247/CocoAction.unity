using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace CocoAction
{
    public class Action
    {
        protected GameObject target { get; set; }

        public virtual bool isDone
        {
            get
            {
                return true;
            }
        }

        public virtual void Start(GameObject target)
        {
            this.target = target;
        }
        public virtual void Update()
        {
        }
    }

    public class FiniteTimeAction : Action
    {
    }
    public class ActionInterval : FiniteTimeAction
    {
        protected float duration { get; set; }
        private int startTick { get; set; }
        
        public override bool isDone
        {
            get
            {
                return Environment.TickCount - startTick >= duration * 1000;
            }
        }

        public override void Start(GameObject target)
        {
            base.Start(target);

            startTick = Environment.TickCount;
        }
    }
    
    public class DelayTime : ActionInterval
    {
        public static DelayTime Create(float delay)
        {
            return new DelayTime(delay);
        }

        private float delay { get; set; }
        private float startTick { get; set; }
        private bool _isDone;

        public override bool isDone
        {
            get
            {
                return _isDone;
            }
        }

        DelayTime(float delay)
        {
            this.delay = delay;
        }

        public override void Start(GameObject target)
        {
            base.Start(target);

            _isDone = false;
            startTick = Environment.TickCount;
        }
        public override void Update()
        {
            if (Environment.TickCount >= startTick + delay * 1000)
                _isDone = true;
        }
    }
    public class CallFunc : FiniteTimeAction
    {
        public static CallFunc Create(Action<GameObject> callback)
        {
            return new CallFunc(callback);
        }

        private Action<GameObject> callback { get; set; }
        private bool _isDone;
        public override bool isDone
        {
            get
            {
                return _isDone;
            }
        }

        CallFunc(Action<GameObject> callback)
        {
            this.callback = callback;
        }

        public override void Start(GameObject target)
        {
            base.Start(target);

            _isDone = false;
        }
        public override void Update()
        {
            callback(target);
            _isDone = true;
        }
    }
    public class Sequence : ActionInterval
    {
        public static Sequence Create(params FiniteTimeAction[] actions)
        {
            return new Sequence(actions);
        }

        private FiniteTimeAction[] actions { get; set; }
        private int cursor { get; set; }

        Sequence(params FiniteTimeAction[] actions)
        {
            this.actions = actions;
        }

        public override bool isDone
        {
            get
            {
                return cursor == actions.Length;
            }
        }

        void MoveToNextAction()
        {
            cursor++;
            
            if (isDone == false)
                actions[cursor].Start(target);
        }
        public override void Start(GameObject target)
        {
            base.Start(target);

            cursor = -1;
            MoveToNextAction();
        }
        public override void Update()
        {
            if (isDone)
                return;
            
            actions[cursor].Update();

            if (actions[cursor].isDone)
                MoveToNextAction();
        }
    }

    public class Repeat : ActionInterval
    {
        public static Repeat Create(ActionInterval action, int count)
        {
            return new Repeat(action, count);
        }

        private int count { get; set; }
        private int cursor { get; set; }
        private ActionInterval action { get; set; }

        public override bool isDone
        {
            get
            {
                return cursor == count && count > 0;
            }
        }

        protected Repeat(ActionInterval action, int count)
        {
            this.action = action;
            this.count = 0;
        }

        public override void Start(GameObject target)
        {
            base.Start(target);

            cursor = 0;
            action.Start(target);
        }
        public override void Update()
        {
            if (action.isDone)
            {
                cursor++;

                if (isDone)
                    return;

                action.Start(target);
            }

            action.Update();
        }
    }
    public class RepeatForever : Repeat
    {
        public static RepeatForever Create(ActionInterval action)
        {
            return new RepeatForever(action);
        }
        public static RepeatForever Create(params ActionInterval[] actions)
        {
            return new RepeatForever(
                Sequence.Create(actions));
        }

        RepeatForever(ActionInterval action) :
            base(action, -1)
        {
        }
    }
    public class RepeatUntil : Repeat
    {
        public static RepeatUntil Create(ActionInterval action, Func<bool> pred)
        {
            return new RepeatUntil(action, pred);
        }
        
        private Func<bool> pred { get; set; }

        public override bool isDone
        {
            get
            {
                return pred() == false;
            }
        }

        RepeatUntil(ActionInterval action, Func<bool> pred) :
            base(action, -1)
        {
            this.pred = pred;
        }
    }

    public class MoveBy : ActionInterval
    {
        public static MoveBy Create(float duration, float x, float y)
        {
            return new MoveBy(duration, x, y);
        }

        public float x { get; set; }
        public float y { get; set; }

        protected MoveBy(float duration, float x, float y)
        {
            this.duration = duration;

            this.x = x;
            this.y = y;
        }

        public override void Start(GameObject target)
        {
            base.Start(target);

            var toX = target.transform.localPosition.x + x;
            var toY = target.transform.localPosition.y + y;

            TweenPosition.Begin(
                target, duration, new Vector3(toX, toY))
                .PlayForward();
        }
    }
    public class MoveTo : MoveBy
    {
        public new static MoveTo Create(float duration, float x, float y)
        {
            return new MoveTo(duration, x, y);
        }

        private float originalX { get; set; }
        private float originalY { get; set; }

        MoveTo(float duration, float x, float y) :
            base(duration, x, y)
        {
            this.originalX = x - target.transform.localPosition.x;
            this.originalX = y - target.transform.localPosition.y;
        }

        public override void Start(GameObject target)
        {
            x = originalX;
            y = originalY;

            base.Start(target);
        }
    }

    public class RotateBy : ActionInterval
    {
        public static RotateBy Create(float duration, float angle)
        {
            return new RotateBy(duration, 0, 0, angle);
        }
        public static RotateBy Create(float duration, float angleX, float angleY, float angleZ)
        {
            return new RotateBy(duration, angleX, angleY, angleZ);
        }

        protected float angleX { get; set; }
        protected float angleY { get; set; }
        protected float angleZ { get; set; }

        protected RotateBy(float duration, float angleX, float angleY, float angleZ)
        {
            this.duration = duration;

            this.angleX = angleX;
            this.angleY = angleY;
            this.angleZ = angleZ;
        }

        public override void Start(GameObject target)
        {
            base.Start(target);

            var toX = angleX + target.transform.localRotation.x;
            var toY = angleY + target.transform.localRotation.y;
            var toZ = angleZ + target.transform.localRotation.z;

            TweenRotation.Begin(
                target, duration, new Quaternion(toX, toY, toZ, 0))
                .PlayForward();
        }
    }
    public class RotateTo : RotateBy
    {
        public new static RotateTo Create(float duration, float angle)
        {
            return new RotateTo(duration, 0,0, angle);
        }
        public new static RotateTo Create(float duration, float angleX, float angleY, float angleZ)
        {
            return new RotateTo(duration, angleX, angleY, angleZ);
        }

        public float originalAngleX { get; set; }
        public float originalAngleY { get; set; }
        public float originalAngleZ { get; set; }

        protected RotateTo(float duration, float angleX, float angleY, float angleZ) :
            base(duration, angleX, angleY, angleZ)
        {
            this.originalAngleX = angleX - target.transform.localRotation.x;
            this.originalAngleY = angleY - target.transform.localRotation.y;
            this.originalAngleZ = angleZ - target.transform.localRotation.z;
        }

        public override void Start(GameObject target)
        {
            angleX = originalAngleX;
            angleY = originalAngleY;
            angleZ = originalAngleZ;

            base.Start(target);
        }
    }

    public class FadeBy : ActionInterval
    {
        public static FadeBy Create(float duration, int opacity)
        {
            return new FadeBy(duration, opacity);
        }

        protected int opacity { get; set; }

        protected FadeBy(float duration, int opacity)
        {
            this.duration = duration;
            this.opacity = opacity;
        }

        public override void Start(GameObject target)
        {
            base.Start(target);

            var toOpacity =
                target.GetComponent<UI2DSprite>().color.a + opacity / 255.0f;

            TweenAlpha.Begin(
                target, duration, toOpacity)
                .PlayForward();
        }
    }
    public class FadeTo : FadeBy
    {
        public new static FadeTo Create(float duration, int opacity)
        {
            return new FadeTo(duration, opacity);
        }

        private int originalOpacity { get; set; }

        FadeTo(float duration, int opacity)
            : base(duration, opacity)
        {
            this.originalOpacity = opacity;
        }

        public override void Start(GameObject target)
        {
            opacity = originalOpacity -
                (int)(target.GetComponent<UI2DSprite>().color.a * 255);

            base.Start(target);
        }
    }
}
