using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESignature
{
    internal class SignedDoc
    {
        public string _userName;
        public byte[] ESignature;
        public string DocContent;
     
        public SignedDoc()
        {
            _userName = "";
            ESignature = new byte[] { };
            DocContent = "";
        }
        public SignedDoc(string fname)
        {
            SignedDoc d;
            if (!LoadFromFile(fname, out d))
                throw new Exception();
            _userName = d._userName;
            ESignature = d.ESignature;
            DocContent = d.DocContent;
        }

        public static bool LoadFromFile(string fname, out SignedDoc doc)
        {
            try
            {
                doc = new SignedDoc();
                using (BinaryReader br = new BinaryReader(new FileStream(fname, FileMode.Open, FileAccess.Read)))
                {
                    int name_len, ESignature_len;
                    string uname;
                    string DocContent; 
                    
                    name_len = br.ReadInt32();
                    ESignature_len = br.ReadInt32();
                    
                    uname = Encoding.ASCII.GetString(br.ReadBytes(name_len));
                    
                    byte[] ESignature = br.ReadBytes(ESignature_len);
                    
                    using (StreamReader sr = new StreamReader(br.BaseStream, Encoding.Unicode))
                    {
                        DocContent = sr.ReadToEnd();
                    }
                    
                    doc.ESignature = ESignature;
                    doc.DocContent = DocContent;
                    doc._userName = uname;
                }
                return true;
            }
            catch
            {
                doc = null;
                return false;
            }
        }

        public void SaveToFile(string fname)
        {
            using (BinaryWriter bw = new BinaryWriter(new FileStream(fname, FileMode.Create, FileAccess.Write)))
            {
                bw.Write(_userName.Length);
                bw.Write(ESignature.Length);
                bw.Write(Encoding.ASCII.GetBytes(_userName));
                bw.Write(ESignature);
                bw.Write(Encoding.Unicode.GetBytes(DocContent));
            }
        }
    }
}
