using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Core
{
	/// <summary>
	/// DES加密解密算法
	/// </summary>
	public class DesHelper
	{
		//默认密钥向量
		private static string iv = "1234567812345678";
		/// <summary>
		/// DES加密字符串
		/// </summary>
		/// <param name="encryptString">待加密的字符串</param>
		/// <param name="encryptKey">加密密钥,要求为8位</param>
		/// <returns>加密成功返回加密后的字符串，失败返回源串</returns>
		public static string EncryptDES(string encryptString, string encryptKey)
		{
			try
			{
				//将字符转换为UTF - 8编码的字节序列
				byte[] rgbKey = Encoding.UTF8.GetBytes(encryptKey.Substring(0, 8));
				byte[] rgbIV = Encoding.UTF8.GetBytes(iv);
				byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
				//用指定的密钥和初始化向量创建CBC模式的DES加密标准
				DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider();
				dCSP.Mode = CipherMode.CBC;
				dCSP.Padding = PaddingMode.PKCS7;
				MemoryStream mStream = new MemoryStream();
				CryptoStream cStream = new CryptoStream(mStream, dCSP.CreateEncryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
				cStream.Write(inputByteArray, 0, inputByteArray.Length);//写入内存流
				cStream.FlushFinalBlock();//将缓冲区中的数据写入内存流，并清除缓冲区
				return Convert.ToBase64String(mStream.ToArray()); //将内存流转写入字节数组并转换为string字符
			}
			catch(Exception ex)
			{
				throw new Exception("加密失败!", ex);
			}
		}

		/// <summary>
		/// DES解密字符串
		/// </summary>
		/// <param name="decryptString">待解密的字符串</param>
		/// <param name="decryptKey">解密密钥,要求为8位,和加密密钥相同</param>
		/// <returns>解密成功返回解密后的字符串，失败返源串</returns>
		public static string DecryptDES(string decryptString, string decryptKey)
		{
			try
			{
				//将字符转换为UTF - 8编码的字节序列
				byte[] rgbKey = Encoding.UTF8.GetBytes(decryptKey.Substring(0, 8));
				byte[] rgbIV = Encoding.UTF8.GetBytes(iv);
				byte[] inputByteArray = Convert.FromBase64String(decryptString);
				//用指定的密钥和初始化向量使用CBC模式的DES解密标准解密
				DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider();
				dCSP.Mode = CipherMode.CBC;
				dCSP.Padding = PaddingMode.PKCS7;
				MemoryStream mStream = new MemoryStream();
				CryptoStream cStream = new CryptoStream(mStream, dCSP.CreateDecryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
				cStream.Write(inputByteArray, 0, inputByteArray.Length);
				cStream.FlushFinalBlock();
				return Encoding.UTF8.GetString(mStream.ToArray());
			}
			catch (Exception ex)
			{
				throw new Exception("解密失败!", ex);
			}
		}
	}
}