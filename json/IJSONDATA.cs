/*libs*/using System;using System.Collections.Generic;using System.Linq;using System.Text;


    public interface IJSONDATA
    {
        T GetData<T>();
        void SetData<T>(T value);
        bool IsData<T>(out T result);
    }

