using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.EnumModel
{
    /// <summary>
    /// 写日志类型
    /// </summary>
    public enum SystemLogTypeEnum
    {
        ///根据需要选择适合的日志级别，以便更好地描述所发生的事件。通常情况下，会将日志级别设置为 Warning 或以上，以确保只有重要的事件才被记录下来。

        /// <summary>
        /// 详细信息，通常用于调试。
        /// </summary>
        Verbose = 1,

        /// <summary>
        /// 调试信息，用于记录程序的内部状态。
        /// </summary>
        Debug = 2,

        /// <summary>
        /// 信息性消息，用于记录程序的运行过程。
        /// </summary>
        Information = 3,

        /// <summary>
        /// 警告信息，表示程序可能出现潜在问题。
        /// </summary>
        Warning = 4,

        /// <summary>
        /// 错误信息，表示程序出现了可处理的异常。
        /// </summary>
        Error = 5,

        /// <summary>
        /// 致命信息，表示程序发生了无法恢复的错误。
        /// </summary>
        Fatal = 6
    }
}
