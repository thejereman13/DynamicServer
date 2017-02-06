﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DynamicServer {
	public class Encryption {

		private readonly RNGCryptoServiceProvider crypt;
		private readonly byte[] key;
		private readonly RijndaelManaged rm;
		private readonly UTF8Encoding encoder;

		public Encryption() {
			crypt = new RNGCryptoServiceProvider();
			rm = new RijndaelManaged();
			encoder = new UTF8Encoding();
			key = Convert.FromBase64String("[REDACTED]");
		}

		public string Encrypt(string unencrypted) {
			var vector = new byte[16];
			crypt.GetBytes(vector);
			var cryptogram = vector.Concat(Encrypt(encoder.GetBytes(unencrypted), vector));
			return Convert.ToBase64String(cryptogram.ToArray());
		}
		public byte[] Encrypt(byte[] unencrypted) {
			var vector = new byte[16];
			crypt.GetBytes(vector);
			var cryptogram = vector.Concat(Encrypt(unencrypted, vector));
			return cryptogram.ToArray();
		}

		public string Decrypt(string encrypted) {
			var cryptogram = Convert.FromBase64String(encrypted);
			if(cryptogram.Length < 17) {
				throw new ArgumentException("Not a valid encrypted string", "encrypted");
			}

			var vector = cryptogram.Take(16).ToArray();
			var buffer = cryptogram.Skip(16).ToArray();
			return encoder.GetString(this.Decrypt(buffer, vector));
		}
		public byte[] Decrypt(byte[] encrypted) {
			var cryptogram = encrypted;
			if(cryptogram.Length < 17) {
				throw new ArgumentException("Not a valid encrypted string", "encrypted");
			}

			var vector = cryptogram.Take(16).ToArray();
			var buffer = cryptogram.Skip(16).ToArray();
			return this.Decrypt(buffer, vector);
		}

		private byte[] Encrypt(byte[] buffer, byte[] vector) {
			var encryptor = rm.CreateEncryptor(this.key, vector);
			return Transform(buffer, encryptor);
		}

		private byte[] Decrypt(byte[] buffer, byte[] vector) {
			var decryptor = rm.CreateDecryptor(this.key, vector);
			return Transform(buffer, decryptor);
		}

		private byte[] Transform(byte[] buffer, ICryptoTransform transform) {
			var stream = new MemoryStream();
			using(var cs = new CryptoStream(stream, transform, CryptoStreamMode.Write)) {
				cs.Write(buffer, 0, buffer.Length);
			}

			return stream.ToArray();
		}

	}
}
