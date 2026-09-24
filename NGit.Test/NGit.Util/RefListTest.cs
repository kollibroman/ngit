/*
This code is derived from jgit (http://eclipse.org/jgit).
Copyright owners are documented in jgit's IP log.

This program and the accompanying materials are made available
under the terms of the Eclipse Distribution License v1.0 which
accompanies this distribution, is reproduced below, and is
available at http://www.eclipse.org/org/documents/edl-v10.php

All rights reserved.

Redistribution and use in source and binary forms, with or
without modification, are permitted provided that the following
conditions are met:

- Redistributions of source code must retain the above copyright
  notice, this list of conditions and the following disclaimer.

- Redistributions in binary form must reproduce the above
  copyright notice, this list of conditions and the following
  disclaimer in the documentation and/or other materials provided
  with the distribution.

- Neither the name of the Eclipse Foundation, Inc. nor the
  names of its contributors may be used to endorse or promote
  products derived from this software without specific prior
  written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND
CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES,
INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT,
STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System;
using System.Text;
using NGit;
using NGit.Util;
using NUnit.Framework.Legacy;
using Sharpen;

namespace NGit.Util
{
	[TestFixture]
	public class RefListTest
	{
		private static readonly ObjectId ID = ObjectId.FromString("41eb0d88f833b558bddeb269b7ab77399cdf98ed"
			);

		private static readonly Ref REF_A = NewRef("A");

		private static readonly Ref REF_B = NewRef("B");

		private static readonly Ref REF_c = NewRef("c");

		[Test]
		public virtual void TestEmpty()
		{
			RefList<Ref> list = RefList.EmptyList();
			Assert.AreEqual(0, list.Size());
			Assert.IsTrue(list.IsEmpty());
			Assert.IsFalse(list.Iterator().HasNext());
			Assert.AreEqual(-1, list.Find("a"));
			Assert.AreEqual(-1, list.Find("z"));
			Assert.IsFalse(list.Contains("a"));
			Assert.IsNull(list.Get("a"));
			try
			{
				list.Get(0);
				Assert.Fail("RefList.emptyList should have 0 element array");
			}
			catch (IndexOutOfRangeException)
			{
			}
		}

		// expected
		[Test]
		public virtual void TestEmptyBuilder()
		{
			RefList<Ref> list = new RefListBuilder<Ref>().ToRefList();
			Assert.AreEqual(0, list.Size());
			Assert.IsFalse(list.Iterator().HasNext());
			Assert.AreEqual(-1, list.Find("a"));
			Assert.AreEqual(-1, list.Find("z"));
			Assert.IsFalse(list.Contains("a"));
			Assert.IsNull(list.Get("a"));
			Assert.IsTrue(list.AsList().IsEmpty());
			Assert.AreEqual("[]", list.ToString());
			// default array capacity should be 16, with no bounds checking.
			Assert.IsNull(list.Get(16 - 1));
			try
			{
				list.Get(16);
				Assert.Fail("default RefList should have 16 element array");
			}
			catch (IndexOutOfRangeException)
			{
			}
		}

		// expected
		[Test]
		public virtual void TestBuilder_AddThenSort()
		{
			RefListBuilder<Ref> builder = new RefListBuilder<Ref>(1);
			builder.Add(REF_B);
			builder.Add(REF_A);
			RefList<Ref> list = builder.ToRefList();
			Assert.AreEqual(2, list.Size());
			Assert.AreSame(REF_B, list.Get(0));
			Assert.AreSame(REF_A, list.Get(1));
			builder.Sort();
			list = builder.ToRefList();
			Assert.AreEqual(2, list.Size());
			Assert.AreSame(REF_A, list.Get(0));
			Assert.AreSame(REF_B, list.Get(1));
		}

		[Test]
		public virtual void TestBuilder_AddAll()
		{
			RefListBuilder<Ref> builder = new RefListBuilder<Ref>(1);
			Ref[] src = new Ref[] { REF_A, REF_B, REF_c, REF_A };
			builder.AddAll(src, 1, 2);
			RefList<Ref> list = builder.ToRefList();
			Assert.AreEqual(2, list.Size());
			Assert.AreSame(REF_B, list.Get(0));
			Assert.AreSame(REF_c, list.Get(1));
		}

		[Test]
		public virtual void TestBuilder_Set()
		{
			RefListBuilder<Ref> builder = new RefListBuilder<Ref>();
			builder.Add(REF_A);
			builder.Add(REF_A);
			Assert.AreEqual(2, builder.Size());
			Assert.AreSame(REF_A, builder.Get(0));
			Assert.AreSame(REF_A, builder.Get(1));
			RefList<Ref> list = builder.ToRefList();
			Assert.AreEqual(2, list.Size());
			Assert.AreSame(REF_A, list.Get(0));
			Assert.AreSame(REF_A, list.Get(1));
			builder.Set(1, REF_B);
			list = builder.ToRefList();
			Assert.AreEqual(2, list.Size());
			Assert.AreSame(REF_A, list.Get(0));
			Assert.AreSame(REF_B, list.Get(1));
		}

		[Test]
		public virtual void TestBuilder_Remove()
		{
			RefListBuilder<Ref> builder = new RefListBuilder<Ref>();
			builder.Add(REF_A);
			builder.Add(REF_B);
			builder.Remove(0);
			Assert.AreEqual(1, builder.Size());
			Assert.AreSame(REF_B, builder.Get(0));
		}

		[Test]
		public virtual void TestSet()
		{
			RefList<Ref> one = ToList(REF_A, REF_A);
			RefList<Ref> two = one.Set(1, REF_B);
			Assert.AreNotSame(one, two);
			// one is not modified
			Assert.AreEqual(2, one.Size());
			Assert.AreSame(REF_A, one.Get(0));
			Assert.AreSame(REF_A, one.Get(1));
			// but two is
			Assert.AreEqual(2, two.Size());
			Assert.AreSame(REF_A, one.Get(0));
			Assert.AreSame(REF_B, two.Get(1));
		}

		[Test]
		public virtual void TestAddToEmptyList()
		{
			RefList<Ref> one = ToList();
			RefList<Ref> two = one.Add(0, REF_B);
			Assert.AreNotSame(one, two);
			// one is not modified, but two is
			Assert.AreEqual(0, one.Size());
			Assert.AreEqual(1, two.Size());
			Assert.IsFalse(two.IsEmpty());
			Assert.AreSame(REF_B, two.Get(0));
		}

		[Test]
		public virtual void TestAddToFrontOfList()
		{
			RefList<Ref> one = ToList(REF_A);
			RefList<Ref> two = one.Add(0, REF_B);
			Assert.AreNotSame(one, two);
			// one is not modified, but two is
			Assert.AreEqual(1, one.Size());
			Assert.AreSame(REF_A, one.Get(0));
			Assert.AreEqual(2, two.Size());
			Assert.AreSame(REF_B, two.Get(0));
			Assert.AreSame(REF_A, two.Get(1));
		}

		[Test]
		public virtual void TestAddToEndOfList()
		{
			RefList<Ref> one = ToList(REF_A);
			RefList<Ref> two = one.Add(1, REF_B);
			Assert.AreNotSame(one, two);
			// one is not modified, but two is
			Assert.AreEqual(1, one.Size());
			Assert.AreSame(REF_A, one.Get(0));
			Assert.AreEqual(2, two.Size());
			Assert.AreSame(REF_A, two.Get(0));
			Assert.AreSame(REF_B, two.Get(1));
		}

		[Test]
		public virtual void TestAddToMiddleOfListByInsertionPosition()
		{
			RefList<Ref> one = ToList(REF_A, REF_c);
			Assert.AreEqual(-2, one.Find(REF_B.GetName()));
			RefList<Ref> two = one.Add(one.Find(REF_B.GetName()), REF_B);
			Assert.AreNotSame(one, two);
			// one is not modified, but two is
			Assert.AreEqual(2, one.Size());
			Assert.AreSame(REF_A, one.Get(0));
			Assert.AreSame(REF_c, one.Get(1));
			Assert.AreEqual(3, two.Size());
			Assert.AreSame(REF_A, two.Get(0));
			Assert.AreSame(REF_B, two.Get(1));
			Assert.AreSame(REF_c, two.Get(2));
		}

		[Test]
		public virtual void TestPutNewEntry()
		{
			RefList<Ref> one = ToList(REF_A, REF_c);
			RefList<Ref> two = one.Put(REF_B);
			Assert.AreNotSame(one, two);
			// one is not modified, but two is
			Assert.AreEqual(2, one.Size());
			Assert.AreSame(REF_A, one.Get(0));
			Assert.AreSame(REF_c, one.Get(1));
			Assert.AreEqual(3, two.Size());
			Assert.AreSame(REF_A, two.Get(0));
			Assert.AreSame(REF_B, two.Get(1));
			Assert.AreSame(REF_c, two.Get(2));
		}

		[Test]
		public virtual void TestPutReplaceEntry()
		{
			Ref otherc = NewRef(REF_c.GetName());
			Assert.AreNotSame(REF_c, otherc);
			RefList<Ref> one = ToList(REF_A, REF_c);
			RefList<Ref> two = one.Put(otherc);
			Assert.AreNotSame(one, two);
			// one is not modified, but two is
			Assert.AreEqual(2, one.Size());
			Assert.AreSame(REF_A, one.Get(0));
			Assert.AreSame(REF_c, one.Get(1));
			Assert.AreEqual(2, two.Size());
			Assert.AreSame(REF_A, two.Get(0));
			Assert.AreSame(otherc, two.Get(1));
		}

		[Test]
		public virtual void TestRemoveFrontOfList()
		{
			RefList<Ref> one = ToList(REF_A, REF_B, REF_c);
			RefList<Ref> two = one.Remove(0);
			Assert.AreNotSame(one, two);
			Assert.AreEqual(3, one.Size());
			Assert.AreSame(REF_A, one.Get(0));
			Assert.AreSame(REF_B, one.Get(1));
			Assert.AreSame(REF_c, one.Get(2));
			Assert.AreEqual(2, two.Size());
			Assert.AreSame(REF_B, two.Get(0));
			Assert.AreSame(REF_c, two.Get(1));
		}

		[Test]
		public virtual void TestRemoveMiddleOfList()
		{
			RefList<Ref> one = ToList(REF_A, REF_B, REF_c);
			RefList<Ref> two = one.Remove(1);
			Assert.AreNotSame(one, two);
			Assert.AreEqual(3, one.Size());
			Assert.AreSame(REF_A, one.Get(0));
			Assert.AreSame(REF_B, one.Get(1));
			Assert.AreSame(REF_c, one.Get(2));
			Assert.AreEqual(2, two.Size());
			Assert.AreSame(REF_A, two.Get(0));
			Assert.AreSame(REF_c, two.Get(1));
		}

		[Test]
		public virtual void TestRemoveEndOfList()
		{
			RefList<Ref> one = ToList(REF_A, REF_B, REF_c);
			RefList<Ref> two = one.Remove(2);
			Assert.AreNotSame(one, two);
			Assert.AreEqual(3, one.Size());
			Assert.AreSame(REF_A, one.Get(0));
			Assert.AreSame(REF_B, one.Get(1));
			Assert.AreSame(REF_c, one.Get(2));
			Assert.AreEqual(2, two.Size());
			Assert.AreSame(REF_A, two.Get(0));
			Assert.AreSame(REF_B, two.Get(1));
		}

		[Test]
		public virtual void TestRemoveMakesEmpty()
		{
			RefList<Ref> one = ToList(REF_A);
			RefList<Ref> two = one.Remove(1);
			Assert.AreNotSame(one, two);
			CollectionAssert.AreEquivalent(two, RefList.EmptyList<Ref>());
		}

		[Test]
		public virtual void TestToString()
		{
			StringBuilder exp = new StringBuilder();
			exp.Append("[");
			exp.Append(REF_A);
			exp.Append(", ");
			exp.Append(REF_B);
			exp.Append("]");
			RefList<Ref> list = ToList(REF_A, REF_B);
			Assert.AreEqual(exp.ToString(), list.ToString());
		}

		[Test]
		public virtual void TestBuilder_ToString()
		{
			StringBuilder exp = new StringBuilder();
			exp.Append("[");
			exp.Append(REF_A);
			exp.Append(", ");
			exp.Append(REF_B);
			exp.Append("]");
			RefListBuilder<Ref> list = new RefListBuilder<Ref>();
			list.Add(REF_A);
			list.Add(REF_B);
			Assert.AreEqual(exp.ToString(), list.ToString());
		}

		[Test]
		public virtual void TestFindContainsGet()
		{
			RefList<Ref> list = ToList(REF_A, REF_B, REF_c);
			Assert.AreEqual(0, list.Find("A"));
			Assert.AreEqual(1, list.Find("B"));
			Assert.AreEqual(2, list.Find("c"));
			Assert.AreEqual(-1, list.Find("0"));
			Assert.AreEqual(-2, list.Find("AB"));
			Assert.AreEqual(-3, list.Find("a"));
			Assert.AreEqual(-4, list.Find("z"));
			Assert.AreSame(REF_A, list.Get("A"));
			Assert.AreSame(REF_B, list.Get("B"));
			Assert.AreSame(REF_c, list.Get("c"));
			Assert.IsNull(list.Get("AB"));
			Assert.IsNull(list.Get("z"));
			Assert.IsTrue(list.Contains("A"));
			Assert.IsTrue(list.Contains("B"));
			Assert.IsTrue(list.Contains("c"));
			Assert.IsFalse(list.Contains("AB"));
			Assert.IsFalse(list.Contains("z"));
		}

		[Test]
		public virtual void TestIterable()
		{
			RefList<Ref> list = ToList(REF_A, REF_B, REF_c);
			int idx = 0;
			foreach (Ref @ref in list)
			{
				Assert.AreSame(list.Get(idx++), @ref);
			}
			Assert.AreEqual(3, idx);
			Iterator<Ref> i = RefList.EmptyList().Iterator();
			try
			{
				i.Next();
				Assert.Fail("did not throw NoSuchElementException");
			}
			catch (NoSuchElementException)
			{
			}
			// expected
			i = list.Iterator();
			Assert.IsTrue(i.HasNext());
			Assert.AreSame(REF_A, i.Next());
			try
			{
				i.Remove();
				Assert.Fail("did not throw UnsupportedOperationException");
			}
			catch (NotSupportedException)
			{
			}
		}

		// expected
		[Test]
		public virtual void TestCopyLeadingPrefix()
		{
			RefList<Ref> one = ToList(REF_A, REF_B, REF_c);
			RefList<Ref> two = one.Copy(2).ToRefList();
			Assert.AreNotSame(one, two);
			Assert.AreEqual(3, one.Size());
			Assert.AreSame(REF_A, one.Get(0));
			Assert.AreSame(REF_B, one.Get(1));
			Assert.AreSame(REF_c, one.Get(2));
			Assert.AreEqual(2, two.Size());
			Assert.AreSame(REF_A, two.Get(0));
			Assert.AreSame(REF_B, two.Get(1));
		}

		[Test]
		public virtual void TestCopyConstructorReusesArray()
		{
			RefListBuilder<Ref> one = new RefListBuilder<Ref>();
			one.Add(REF_A);
			RefList<Ref> two = new RefList<Ref>(one.ToRefList());
			one.Set(0, REF_B);
			Assert.AreSame(REF_B, two.Get(0));
		}

		private RefList<Ref> ToList(params Ref[] refs)
		{
			RefListBuilder<Ref> b = new RefListBuilder<Ref>(refs.Length);
			b.AddAll(refs, 0, refs.Length);
			return b.ToRefList();
		}

		private static Ref NewRef(string name)
		{
			return new ObjectIdRef.Unpeeled(RefStorage.LOOSE, name, ID);
		}
	}
}
