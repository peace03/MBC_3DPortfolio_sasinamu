using System;

public static class EventBus<T> where T : IEvent
{
    public static event Action<T> OnEvent;      // 이벤트를 수신할 구독자 명단

    // 특정 이벤트 발생을 알려주고 데이터 전달하는 함수
    public static void Publish(T data) => OnEvent?.Invoke(data);
}