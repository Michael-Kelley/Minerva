#region Includes

using System;

#endregion

namespace Minerva
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public class TelnetCommandAttribute : Attribute
	{
		#region Private Fields

		string _name;
		string _desc;
		string[] _parameters = null;

		#endregion

		#region Constructors

		public TelnetCommandAttribute(string name, string desc)
		{
			_name = name;
			_desc = desc;
		}

		public TelnetCommandAttribute(string name, string desc, params string[] paramDescs)
		{
			_name = name;
			_desc = desc;
			_parameters = paramDescs;
		}

		#endregion

		#region Accessors

		public string Name { get { return _name; } }
		public string Description { get { return _desc; } }
		public string[] ParameterDescriptions { get { return _parameters; } }

		#endregion
	}

	public class TelnetCommandDescriptor
	{
		#region Private Fields

		string _name;
		string _desc;
		string[] _paramNames;
		string[] _paramDescs;
		string _header;
		string _descFull;

		#endregion

		#region Constructor

		public TelnetCommandDescriptor(string name, string desc, string[] paramNames, string[] paramDescs)
		{
			_name = name;
			_desc = desc;
			_paramNames = paramNames;
			_paramDescs = paramDescs;

			var body = "";
			var sParams = "";

			int c = paramNames.Length;

			for (int i = 0; i < c; i++)
			{
				if (i != 0)
					sParams += ", ";

				sParams += paramNames[i];
				body += (i == c - 1) ? String.Format("\t{0}\t\t{1}", paramNames[i], paramDescs[i]) :
									   String.Format("\t{0}\t\t{1}\n", paramNames[i], paramDescs[i]);
			}

			_header = (sParams != "") ? String.Format("{0} {{{1}}} - {2}", name, sParams, desc) :
											   String.Format("{0} - {1}", name, desc);
			_descFull = (c == 0) ? String.Format("{0}{1}", _header, body) :
										  String.Format("{0}\n\n{1}", _header, body);
		}

		#endregion

		#region Accessors

		public string Name { get { return _name; } }
		public string Description { get { return _desc; } }
		public string[] ParameterNames { get { return _paramNames; } }
		public string[] ParamDescriptions { get { return _paramDescs; } }
		public string FunctionHeader { get { return _header; } }
		public string FullDescription { get { return _descFull; } }

		#endregion
	}
}