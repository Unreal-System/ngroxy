﻿#region summary

//   ------------------------------------------------------------------------------------------------
//   <copyright file="SocksServerHandler.cs">
//     用户：朱宏飞
//     日期：2017/02/05
//     时间：2:47
//   </copyright>
//   ------------------------------------------------------------------------------------------------

#endregion

using System;
using DotNetty.Buffers;
using DotNetty.Common.Internal.Logging;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.Logging;
using Ngroxy.Handlers.Socks.V4;
using Ngroxy.Handlers.Socks.V5;

namespace Ngroxy.Handlers.Socks
{
    public class SocksServerHandler : ChannelHandlerAdapter
    {
        private static ILogger _logger = InternalLoggerFactory.DefaultFactory.CreateLogger<SocksServerHandler>();
        
        /// <inheritdoc />
        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            var buffer = message as IByteBuffer;
            if (buffer == null) return;

            switch (buffer.GetByte(buffer.ReaderIndex))
            {
                case SocksProtocolVersion.Socks4A:
                    context.Channel.Pipeline.Replace(this, nameof(Socks4ServerHandler), new Socks4ServerHandler());
                    break;
                case SocksProtocolVersion.Socks5:
                    context.Channel.Pipeline.Replace(this, nameof(Socks5ServerHandler), new Socks5ServerHandler());
                    break;
                default:
                    _logger.LogError("未知socks版本协议.");
                    break;
            }
            context.FireChannelRead(message);
        }
    }
}