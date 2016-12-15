/*
 * CoroutineSequence.cs
 * 
 * Copyright (c) 2016 Kazunori Tamura
 * Modified (c) 2016 Citrine
 * This software is released under the MIT License.
 * http://opensource.org/licenses/mit-license.php
 */

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// コルーチンを組み合わせて実行するためのSequenceクラス
/// </summary>
public class CoroutineSequence
{
    /// <summary>
    /// Insertで追加されたEnumeratorを管理するクラス
    /// </summary>
    private class InsertedEnumerator
    {
        /// <summary>
        /// 位置
        /// </summary>
        private float _atPosition;

        /// <summary>
        /// 内部のIEnumerator
        /// </summary>
        public IEnumerator InternalEnumerator { get; private set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public InsertedEnumerator(float atPosition, IEnumerator enumerator)
        {
            _atPosition = atPosition;
            InternalEnumerator = enumerator;
        }

        /// <summary>
        /// Enumeratorの取得
        /// </summary>
        public IEnumerator GetEnumerator(Action callback)
        {
            if (_atPosition > 0f)
            {
                yield return new WaitForSeconds(_atPosition);
            }
            yield return InternalEnumerator;
			callback?.Invoke();
		}
    }

    /// <summary>
    /// Insertされたenumerator
    /// </summary>
    private List<InsertedEnumerator> _insertedEnumerators;

    /// <summary>
    /// Appendされたenumerator
    /// </summary>
    private List<IEnumerator> _appendedEnumerators;

    /// <summary>
    /// 終了時に実行するAction
    /// </summary>
    private Action _onCompleted;

    /// <summary>
    /// コルーチンの実行者
    /// </summary>
    private BaseBehavior _owner;

    /// <summary>
    /// 内部で実行されたコルーチンのリスト
    /// </summary>
    private List<Coroutine> _coroutines;

    /// <summary>
    /// 追加されたCoroutineSequenceのリスト
    /// </summary>
    private List<CoroutineSequence> _sequences;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public CoroutineSequence(BaseBehavior owner)
    {
        _owner = owner;
        _insertedEnumerators = new List<InsertedEnumerator>();
        _appendedEnumerators = new List<IEnumerator>();
        _coroutines = new List<Coroutine>();
        _sequences = new List<CoroutineSequence>();
    }

    /// <summary>
    /// enumeratorをatPositionにInsertする
    /// atPosition秒後にenumeratorが実行される
    /// </summary>
    public CoroutineSequence Insert(float atPosition, IEnumerator enumerator)
    {
        _insertedEnumerators.Add(new InsertedEnumerator(atPosition, enumerator));
        return this;
    }

    /// <summary>
    /// CoroutineSequenceをatPositionにInsertする
    /// </summary>
    public CoroutineSequence Insert(float atPosition, CoroutineSequence sequence)
    {
        _insertedEnumerators.Add(new InsertedEnumerator(atPosition, sequence.GetEnumerator()));
        _sequences.Add(sequence);
        return this;
    }

    /// <summary>
    /// callbackをatPositionにInsertする
    /// </summary>
    public CoroutineSequence InsertCallback(float atPosition, Action callback)
    {
        _insertedEnumerators.Add(new InsertedEnumerator(atPosition, GetCallbackEnumerator(callback)));
        return this;
    }

    /// <summary>
    /// enumeratorをAppendする
    /// Appendされたenumeratorは、Insertされたenumeratorが全て実行された後に順番に実行される
    /// </summary>
    public CoroutineSequence Append(IEnumerator enumerator)
    {
        _appendedEnumerators.Add(enumerator);
        return this;
    }

    /// <summary>
    /// CoroutineSequenceをAppendする
    /// </summary>
    public CoroutineSequence Append(CoroutineSequence sequence)
    {
        _appendedEnumerators.Add(sequence.GetEnumerator());
        _sequences.Add(sequence);
        return this;
    }

    /// <summary>
    /// callbackをAppendする
    /// </summary>
    public CoroutineSequence AppendCallback(Action callback)
    {
        _appendedEnumerators.Add(GetCallbackEnumerator(callback));
        return this;
    }

    /// <summary>
    /// 待機をAppendする
    /// </summary>
    public CoroutineSequence AppendInterval(float seconds)
    {
        _appendedEnumerators.Add(GetWaitForSecondsEnumerator(seconds));
        return this;
    }

    /// <summary>
    /// 終了時の処理を追加する
    /// </summary>
    public CoroutineSequence OnCompleted(Action action)
    {
        _onCompleted += action;
        return this;
    }

    /// <summary>
    /// シーケンスを実行する
    /// </summary>
    public Coroutine Play()
    {
        Coroutine coroutine = _owner.StartCoroutine(GetEnumerator());
        _coroutines.Add(coroutine);
        return coroutine;
    }

    /// <summary>
    /// シーケンスを止める
    /// </summary>
    public void Stop()
    {
        foreach (Coroutine coroutine in _coroutines)
        {
            _owner.StopCoroutine(coroutine);
        }
        foreach (InsertedEnumerator insertedEnumerator in _insertedEnumerators)
        {
            _owner.StopCoroutine(insertedEnumerator.InternalEnumerator);
        }
        foreach (IEnumerator enumerator in _appendedEnumerators)
        {
            _owner.StopCoroutine(enumerator);
        }
        foreach (CoroutineSequence sequence in _sequences)
        {
            sequence.Stop();
        }
        _coroutines.Clear();
        _insertedEnumerators.Clear();
        _appendedEnumerators.Clear();
        _sequences.Clear();
    }

    /// <summary>
    /// callbackを実行するIEnumeratorを取得する
    /// </summary>
    private IEnumerator GetCallbackEnumerator(Action callback)
    {
        callback();
        yield break;
    }

    /// <summary>
    /// seconds秒待機するIEnumeratorを取得する
    /// </summary>
    private IEnumerator GetWaitForSecondsEnumerator(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }

    /// <summary>
    /// シーケンスのIEnumeratorを取得する
    /// </summary>
    private IEnumerator GetEnumerator()
    {
        // InsertされたIEnumeratorの実行
        int counter = _insertedEnumerators.Count;
        foreach (InsertedEnumerator insertedEnumerator in _insertedEnumerators)
        {
            Coroutine coroutine = _owner.StartCoroutine(insertedEnumerator.GetEnumerator(() =>
            {
                counter--;
            }));
            _coroutines.Add(coroutine);
        }
        // InsertされたIEnumeratorが全て実行されるのを待つ
        while (counter > 0)
        {
            yield return null;
        }
        // AppendされたIEnumeratorの実行
        foreach (IEnumerator appendedEnumerator in _appendedEnumerators)
        {
            yield return appendedEnumerator;
        }
		// 終了時の処理
		_onCompleted?.Invoke();
	}

	/// <summary>
	/// コルーチンを実行する <see cref="BaseBehavior"/> を取得します。
	/// </summary>
	public BaseBehavior Owner => _owner;

}

/// <summary>
/// <see cref="CoroutineSequence"/> に、<see cref="BaseBehavior"/> APIへのアクセス機能を追加します。
/// </summary>
public static class CoroutineSequenceApiExtension
{
	public static CoroutineSequence Lock(this CoroutineSequence cs) => cs.AppendCallback(() => cs.Owner.Lock());
	public static CoroutineSequence UnLock(this CoroutineSequence cs) => cs.AppendCallback(() => cs.Owner.UnLock());
	public static CoroutineSequence PlaySound(this CoroutineSequence cs, string path) => cs.AppendCallback(() => cs.Owner.PlaySound(path));
	public static CoroutineSequence MessageAndNod(this CoroutineSequence cs, string message, string name = null) => cs.Append(cs.Owner.MessageAndNod(message, name));
	public static CoroutineSequence Message(this CoroutineSequence cs, string message, string name = null) => cs.Append(cs.Owner.Message(message, name));
	public static CoroutineSequence Nod(this CoroutineSequence cs) => cs.Append(cs.Owner.Nod());
	public static CoroutineSequence ShowBox(this CoroutineSequence cs) => cs.AppendCallback(() => cs.Owner.ShowBox());
	public static CoroutineSequence HideBox(this CoroutineSequence cs) => cs.AppendCallback(() => cs.Owner.HideBox());
	public static CoroutineSequence ChangeMap(this CoroutineSequence cs, MapData mapData, Vector3 pos, float angle, float time = 2f) => cs.Append(cs.Owner.ChangeMap(mapData, pos, angle, time));
	public static CoroutineSequence FadeIn(this CoroutineSequence cs, float time) => cs.Append(cs.Owner.FadeIn(time));
	public static CoroutineSequence FadeOut(this CoroutineSequence cs, float time) => cs.Append(cs.Owner.FadeOut(time));

	//public static CoroutineSequence LockAsync(this CoroutineSequence cs, float time = 0) => cs.InsertCallback(time, () => cs.Owner.Lock());
	//public static CoroutineSequence UnLockAsync(this CoroutineSequence cs, float time = 0) => cs.InsertCallback(time, () => cs.Owner.UnLock());
	//public static CoroutineSequence PlaySoundAsync(this CoroutineSequence cs, string path, float time = 0) => cs.InsertCallback(time, () => cs.Owner.PlaySound(path));
	//public static CoroutineSequence MessageAndNodAsync(this CoroutineSequence cs, string message, string name = null, float time = 0) => cs.Insert(time, cs.Owner.MessageAndNod(message, name));
	//public static CoroutineSequence MessageAsync(this CoroutineSequence cs, string message, string name = null, float time = 0) => cs.Insert(time, cs.Owner.Message(message, name));
	//public static CoroutineSequence NodAsync(this CoroutineSequence cs, float time = 0) => cs.Insert(time, cs.Owner.Nod());
	//public static CoroutineSequence ShowBoxAsync(this CoroutineSequence cs, float time = 0) => cs.InsertCallback(time, () => cs.Owner.ShowBox());
	//public static CoroutineSequence HideBoxAsync(this CoroutineSequence cs, float time = 0) => cs.InsertCallback(time, () => cs.Owner.HideBox());
	//public static CoroutineSequence ChangeMapAsync(this CoroutineSequence cs, MapData mapData, Vector3 pos, float angle, float time = 2f, float delayTime = 0) => cs.Insert(delayTime, cs.Owner.ChangeMap(mapData, pos, angle, time));

}