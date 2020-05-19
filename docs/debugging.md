# Debugging

For bluetooth debug purposes there is a debug tab that permits to see the device identifier, contacted devices information, and manipulate the database information (explicitly add, clear).
This tab is conditionally triggered on or off based on the `ANALYTICS` or `DEBUG` flag in a project specific prism platform initializer:
```cs
    public void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
    {
#if DEBUG || ANALYTICS
        moduleCatalog.AddModule<Covi.Features.Debugging.DebuggingModule>(InitializationMode.WhenAvailable);
#endif
    }
```

In the platform specific entry points search for the following lines
```cs
#if DEBUG || ANALYTICS
    if (!string.IsNullOrWhiteSpace(Constants.AppCenterConstants.Secret_iOS))
    {
        Microsoft.AppCenter.AppCenter.Start(Constants.AppCenterConstants.Secret_iOS,
            typeof(Microsoft.AppCenter.Analytics.Analytics),
            typeof(Microsoft.AppCenter.Crashes.Crashes));
            Covi.Logs.Logger.Factory.AddProvider(new Covi.iOS.Services.Log.AppCenterLogProvider());
    }
#endif
```
These conditional flags enables crash reporting and analytics (if AppCenter key is provided), but also 
logging to console and AppCenter (throe analytics feature)  (logs are very extensive and protocol everything in the app, keep in mind when on metered connection)
``` cs
Covi.Logs.Logger.Factory.AddProvider(new Covi.iOS.Services.Log.AppCenterLogProvider());
```
