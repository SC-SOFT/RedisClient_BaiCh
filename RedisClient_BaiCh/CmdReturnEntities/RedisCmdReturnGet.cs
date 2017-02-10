namespace RedisClient_BaiCh.CmdReturnEntities
{
    public class RedisCmdReturnGet: RedisCmdReturn
    {
        /// <summary>
        /// 获取到的数据，空字符串表示指定的键不存在
        /// </summary>
        public string Data { get; set; }

        public RedisCmdReturnGet(CommandMethodReturn commandMethodReturn) : base(commandMethodReturn)
        {
            Data = commandMethodReturn.BulkStrings;
        }
    }
}
