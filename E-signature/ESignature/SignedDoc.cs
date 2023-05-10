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
        
        public string _userName; // TODO Remove
        public byte[] cert;
        public byte[] ESignature;
        public string DocContent;
     
        public SignedDoc()
        {
            _userName = "";
            ESignature = new byte[] { };
            DocContent = "";
            cert = new byte[] { };
        }
        public SignedDoc(string fname)
        {
            SignedDoc d;
            if (!LoadFromFile(fname, out d))
                throw new Exception();
            _userName = d._userName;
            cert = d.cert;
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
                    int name_len, ESignature_len, cert_len;
                    string uname;
                    string DocContent;

                    //name_len = br.ReadInt32();
                    cert_len = br.ReadInt32();
                    ESignature_len = br.ReadInt32();

                    byte[] certificate = br.ReadBytes(cert_len);
                    byte[] ESignature = br.ReadBytes(ESignature_len);
                    
                    using (StreamReader sr = new StreamReader(br.BaseStream, Encoding.Unicode))
                    {
                        DocContent = sr.ReadToEnd();
                    }
                    
                    doc.ESignature = ESignature;
                    doc.DocContent = DocContent;
                    doc.cert = certificate;
                    //doc._userName = uname;
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
                //bw.Write(_userName.Length);
                bw.Write(cert.Length);
                bw.Write(ESignature.Length);
                //bw.Write(Encoding.ASCII.GetBytes(_userName));
                bw.Write(cert);
                bw.Write(ESignature);
                bw.Write(Encoding.Unicode.GetBytes(DocContent));
            }
        }
    }
}
