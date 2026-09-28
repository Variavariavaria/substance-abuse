using Godot;
using System;
using System.Security.Cryptography;
using System.Text;

public partial class RNG : RefCounted
{
	private RandomNumberGenerator _rng = new();

	public void SetSeed(string stringSeed)
	{
		byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(stringSeed));
		_rng.Seed = BitConverter.ToUInt64(hash, 0);
	}

	public int Range(int min, int max) => _rng.RandiRange(min, max);
	public float RangeF(float min, float max) => _rng.RandfRange(min, max);

	public T PickWeighted<T>(System.Collections.Generic.IReadOnlyList<(T item, float weight)> pool)
	{
		float total = 0f;
		foreach (var (_, w) in pool) total += w;
		float roll = _rng.RandfRange(0f, total);
		float acc = 0f;
		foreach (var (item, w) in pool)
		{
			acc += w;
			if (roll <= acc) return item;
		}
		return pool[^1].item;
	}
}
