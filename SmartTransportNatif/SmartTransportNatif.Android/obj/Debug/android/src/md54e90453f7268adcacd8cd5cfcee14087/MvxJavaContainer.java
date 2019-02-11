package md54e90453f7268adcacd8cd5cfcee14087;


public class MvxJavaContainer
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("MvvmCross.Platform.Droid.MvxJavaContainer, MvvmCross.Platform.Droid, Version=5.0.5.0, Culture=neutral, PublicKeyToken=null", MvxJavaContainer.class, __md_methods);
	}


	public MvxJavaContainer () throws java.lang.Throwable
	{
		super ();
		if (getClass () == MvxJavaContainer.class)
			mono.android.TypeManager.Activate ("MvvmCross.Platform.Droid.MvxJavaContainer, MvvmCross.Platform.Droid, Version=5.0.5.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
