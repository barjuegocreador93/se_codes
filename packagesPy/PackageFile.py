

#!/usr/bin/env python
# -*- coding: utf-8 -*-

class PackageFile():

    def __init__(self,files, fileResult):
        self.files = files
        self.file = fileResult
        pass

    def Run(self):
        code = []
        for i in self.files:

            d = open(i, encoding='utf-8-sig')
            folder =".."
            for fs in i.split('/'):
                if fs.__contains__('..'):
                    fs = fs.replace('..','')
                    folder += fs
                elif( not fs.__contains__('.cs')):
                    folder+="/"+fs
            lines=""
            ok = True
            flin = d.readlines()
            for l in flin:
                st = str(l)

                if(st.__contains__("//add-")):
                    fs = st.split('-')[1].replace('\n',' ')
                    self.files.append(folder+"/"+fs)
                    print(folder+"/"+fs)
                if(not st.__contains__("/*libs*/") and not st.__contains__("//add-")):
                    lines+= st

            code.append(lines)
            d.close()

        f = open(self.file,'w', encoding="utf-8")
        for i in code:
            print(i)
            f.write(str(i))
        f.close()