# Introduction #

https protocol is required in order to be able to commit to SVN repository.


# Details #

If the checkout was done using http://gccv2.googlecode.com/svn/trunk, the commit will fail wit following error (after few retries)


```
Command: Commit
Error: Commit failed (details follow):
Error: MKACTIVITY of '/svn/!svn/act/de4a2723-0adb-7e44-9f60-11dd2de2d255':
Error: authorization failed: Could not authenticate to server: rejected Basic
Error: challenge (http://gccv2.googlecode.com)
```


https protocol is required in order to be able to commit. I.E. the local checkout **must** be done using https://gccv2.googlecode.com/svn/trunk