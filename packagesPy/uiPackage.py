#!/usr/bin/env python
# -*- coding: utf-8 -*-
from packagesPy.PackageFile import PackageFile

F = PackageFile(["../BasicCode/UIexample.cs","../ui/ui.cs","../AppBase.cs"],"../Results/ui.cs")
F.Run()