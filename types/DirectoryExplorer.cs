using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HomeTrack
{
	public class DirectoryExplorer
	{
		private readonly Stack<string> _path;
		private DirectoryInfo _directory;

		public DirectoryExplorer(string root)
		{
			_directory = new DirectoryInfo(root);
			_path = new Stack<string>();
			_path.Push("");
		}

		public string Name
		{
			get
			{
				return _path.Count == 1
				       	? "/"
				       	: string.Join("/", _path.Reverse());
			}
		}

		public bool NavigateTo(string path)
		{
			DirectoryInfo current = _directory;
			var names = new List<string>();

			foreach (string name in path.Split('/'))
			{
				current = current.GetDirectories().SingleOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
				if (current == null)
					return false;

				names.Add(current.Name);
			}

			_directory = current;
			names.ForEach(_path.Push);

			return true;
		}

		public IEnumerable<DirectoryInfo> GetDirectories()
		{
			return _directory.GetDirectories();
		}

		public IEnumerable<FileInfo> GetFiles()
		{
			return _directory.GetFiles();
		}

		public string GetFilename(string filename)
		{
			return Path.Combine(_directory.FullName, filename);
		}
	}
}