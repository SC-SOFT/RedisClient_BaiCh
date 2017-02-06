# RedisClient_BaiCh
目标是一个简单易用的Redis客户端</br>
支持异步执行，支持密码验证

通过代码实例化
```c#
RedisClient _redisClient = new RedisClient("127.0.0.1", 6379,"7654321")
```

通过配置文件配置实例化
```xml
<configuration>
  <configSections>
    <section name="RedisClient_BaiCh" type="RedisClient_BaiCh.Config.RedisClient_BaiChSection,RedisClient_BaiCh"/>
  </configSections>

  <RedisClient_BaiCh>
    <Connection Server="192.168.25.171" Port="6379" Auth="7654321"></Connection>
  </RedisClient_BaiCh>
</configuration>
```
执行命令
```c#
_redisClient.Command("keys *");
_redisClient.GetAsync(<keyname>);
...
```
