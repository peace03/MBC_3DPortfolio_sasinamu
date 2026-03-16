using System;
using System.Collections.Generic;

public static class Subject<T> where T : class
{
    private static List<T> observers = new();      // 이벤트(인터페이스) 구독자들 리스트

    // 구독 함수
    public static void Attach(T obs) => observers.Add(obs);

    // 구독 해제 함수
    public static void Detach(T obs) => observers.Remove(obs);

    // 이벤트 발행 함수
    public static void Publish(Action<T> action)
    {
        // 구독자 리스트의 수만큼
        foreach (var obs in observers)
            // 이벤트 발행(구독자 obs에 있는 action 함수 실행)
            action?.Invoke(obs);
    }
}