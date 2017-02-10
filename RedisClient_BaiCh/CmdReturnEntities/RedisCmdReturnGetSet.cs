namespace RedisClient_BaiCh.CmdReturnEntities
{
    public class RedisCmdReturnGetSet : RedisCmdReturn
    {
        /// <summary>
        /// 更新前的值，空字符串表示指定的键不存在或原值为空
        /// </summary>
        public string Data { get; set; }

        public RedisCmdReturnGetSet(CommandMethodReturn commandMethodReturn) : base(commandMethodReturn)
        {
            Data = commandMethodReturn.BulkStrings;
        }
    }
}
