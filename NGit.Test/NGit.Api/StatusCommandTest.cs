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

using System.Collections.Generic;
using NGit.Api;
using NGit.Api.Errors;
using NGit.Errors;
using NUnit.Framework.Legacy;
using Sharpen;

namespace NGit.Test.NGit.Api
{
	[TestFixture]
	public class StatusCommandTest : RepositoryTestCase
	{
		/// <exception cref="NoWorkTreeException"></exception>
		/// <exception cref="GitAPIException"></exception>
		[Test]
		public virtual void TestEmptyStatus()
		{
			Git git = new Git(db);
			Status stat = git.Status().Call();
			Assert.AreEqual(0, stat.GetAdded().Count);
			Assert.AreEqual(0, stat.GetChanged().Count);
			Assert.AreEqual(0, stat.GetMissing().Count);
			Assert.AreEqual(0, stat.GetModified().Count);
			Assert.AreEqual(0, stat.GetRemoved().Count);
			Assert.AreEqual(0, stat.GetUntracked().Count);
		}

		/// <exception cref="System.IO.IOException"></exception>
		/// <exception cref="NoFilepatternException"></exception>
		/// <exception cref="GitAPIException"></exception>
		[Test]
		public virtual void TestDifferentStates()
		{
			Git git = new Git(db);
			WriteTrashFile("a", "content of a");
			WriteTrashFile("b", "content of b");
			WriteTrashFile("c", "content of c");
			git.Add().AddFilepattern("a").AddFilepattern("b").Call();
			Status stat = git.Status().Call();
			Assert.AreEqual(Set("a", "b"), Set (stat.GetAdded()));
			Assert.AreEqual(0, stat.GetChanged().Count);
			Assert.AreEqual(0, stat.GetMissing().Count);
			Assert.AreEqual(0, stat.GetModified().Count);
			Assert.AreEqual(0, stat.GetRemoved().Count);
			Assert.AreEqual(Set("c"), Set (stat.GetUntracked()));
			git.Commit().SetMessage("initial").Call();
			WriteTrashFile("a", "modified content of a");
			WriteTrashFile("b", "modified content of b");
			WriteTrashFile("d", "content of d");
			git.Add().AddFilepattern("a").AddFilepattern("d").Call();
			WriteTrashFile("a", "again modified content of a");
			stat = git.Status().Call();
			Assert.AreEqual(Set("d"), Set (stat.GetAdded()));
			Assert.AreEqual(Set("a"), Set (stat.GetChanged()));
			Assert.AreEqual(0, stat.GetMissing().Count);
			CollectionAssert.AreEquivalent(Set("b", "a"), Set (stat.GetModified()));
			Assert.AreEqual(0, stat.GetRemoved().Count);
			Assert.AreEqual(Set("c"), Set (stat.GetUntracked()));
			git.Add().AddFilepattern(".").Call();
			git.Commit().SetMessage("second").Call();
			stat = git.Status().Call();
			Assert.AreEqual(0, stat.GetAdded().Count);
			Assert.AreEqual(0, stat.GetChanged().Count);
			Assert.AreEqual(0, stat.GetMissing().Count);
			Assert.AreEqual(0, stat.GetModified().Count);
			Assert.AreEqual(0, stat.GetRemoved().Count);
			Assert.AreEqual(0, stat.GetUntracked().Count);
			DeleteTrashFile("a");
			Assert.IsFalse(new FilePath(git.GetRepository().WorkTree, "a").Exists
				());
			git.Add().AddFilepattern("a").SetUpdate(true).Call();
			WriteTrashFile("a", "recreated content of a");
			stat = git.Status().Call();
			Assert.AreEqual(0, stat.GetAdded().Count);
			Assert.AreEqual(0, stat.GetChanged().Count);
			Assert.AreEqual(0, stat.GetMissing().Count);
			Assert.AreEqual(0, stat.GetModified().Count);
			Assert.AreEqual(Set("a"), Set (stat.GetRemoved()));
			Assert.AreEqual(Set("a"), Set (stat.GetUntracked()));
			git.Commit().SetMessage("t").Call();
		}

		public static StringSet Set(params string[] elements)
		{
			return new StringSet (elements);
		}
		
		public static StringSet Set(IEnumerable<string> elements)
		{
			return new StringSet (elements);
		}
		
		public class StringSet : IEnumerable<string>
		{
			HashSet<string> sset;

			public StringSet (IEnumerable<string> s)
			{
				sset = new HashSet<string>(s);
			}

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
			{
				return sset.GetEnumerator ();
			}

			IEnumerator<string> IEnumerable<string>.GetEnumerator ()
			{
				return sset.GetEnumerator ();
			}

			public override bool Equals (object obj)
			{
				StringSet s = obj as StringSet;
				if (s == null)
					return false;
				if (sset.Count != s.sset.Count)
					return false;
				foreach (var v in sset)
					if (!s.sset.Contains (v))
						return false;
				return true;
			}
			
			public override int GetHashCode ()
			{
				int c = 0;
				foreach (var s in sset) {
					unchecked {
						c += s.GetHashCode ();
					}
				}
				return c;
			}
		}
	}
}
