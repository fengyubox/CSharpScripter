﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.IO;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

namespace CSharpScripter {
   public partial class FormMain : Form {
      public FormMain() {
         InitializeComponent();
         this.tbxCode.Document.HighlightingStrategy = HighlightingStrategyFactory.CreateHighlightingStrategy("C#");

         Console.SetOut(new TextBoxWriter(this.tbxRun));
      }

      private void Run() {
         this.SaveCode();

         // 코드
         string code = this.tbxCode.Text;

         // 컴파일러 생성
         CodeDomProvider codeDom = CodeDomProvider.CreateProvider("CSharp");
         // 컴파일 파라미터
         CompilerParameters cparams = new CompilerParameters();
         cparams.GenerateInMemory = true;
         // 컴파일
         CompilerResults results = codeDom.CompileAssemblyFromSource(cparams, code);

         // 출력 메시지
         foreach (var result in results.Output) {
            Console.WriteLine(result);
         }

         if (results.Errors.Count != 0) {
            Console.WriteLine("===============================");
            return;
         }

         // 에러 없으면
         // 어셈블리 로딩
         Type startClass = results.CompiledAssembly.GetType("Test");
         if (startClass == null) {
            Console.WriteLine("StartUp Class must be \"Test\"");
            Console.WriteLine("===============================");
            return;
         }
         // 메인함수 실행
         var startMethod = startClass.GetMethod("Main");
         if (startMethod == null) {
            Console.WriteLine("StartUp Method must be \"Main\"");
            Console.WriteLine("===============================");
            return;
         }
         startMethod.Invoke(null, new object[0]);
         Console.WriteLine("===============================");
      }

      private void btnRun_Click(object sender, EventArgs e) {
         this.Run();
      }

      private void SaveCode() {
         Properties.Settings.Default.code = this.tbxCode.Text;
         Properties.Settings.Default.Save();
      }

      private void LoadCode() {
         Properties.Settings.Default.Reload();
         this.tbxCode.Text = Properties.Settings.Default.code;
      }

      private void btnClear_Click(object sender, EventArgs e) {
         this.tbxRun.Clear();
      }

      private void btnSave_Click(object sender, EventArgs e) {
         this.SaveCode();
      }

      private void btnLoad_Click(object sender, EventArgs e) {
         this.LoadCode();
      }

      private void FormMain_Load(object sender, EventArgs e) {
         this.LoadCode();
      }

      private void FormMain_FormClosing(object sender, FormClosingEventArgs e) {
         this.SaveCode();
      }
   }
}
