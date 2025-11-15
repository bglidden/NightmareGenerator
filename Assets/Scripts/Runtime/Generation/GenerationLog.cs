using System.Text;
using UnityEngine;

namespace AHP
{
    public class GenerationLog
    {
        private StringBuilder _sb = new StringBuilder();

        public void Info(string msg) => _sb.AppendLine($"[INFO] {msg}");
        public void Warn(string msg) => _sb.AppendLine($"[WARN] {msg}");
        public void Error(string msg) => _sb.AppendLine($"[ERROR] {msg}");

        public void Clear() => _sb.Clear();

        public override string ToString() => _sb.ToString();
    }
}