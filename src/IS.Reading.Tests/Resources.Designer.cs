﻿//------------------------------------------------------------------------------
// <auto-generated>
//     O código foi gerado por uma ferramenta.
//     Versão de Tempo de Execução:4.0.30319.42000
//
//     As alterações ao arquivo poderão causar comportamento incorreto e serão perdidas se
//     o código for gerado novamente.
// </auto-generated>
//------------------------------------------------------------------------------

namespace IS.Reading {
    using System;
    
    
    /// <summary>
    ///   Uma classe de recurso de tipo de alta segurança, para pesquisar cadeias de caracteres localizadas etc.
    /// </summary>
    // Essa classe foi gerada automaticamente pela classe StronglyTypedResourceBuilder
    // através de uma ferramenta como ResGen ou Visual Studio.
    // Para adicionar ou remover um associado, edite o arquivo .ResX e execute ResGen novamente
    // com a opção /str, ou recrie o projeto do VS.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Retorna a instância de ResourceManager armazenada em cache usada por essa classe.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("IS.Reading.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Substitui a propriedade CurrentUICulture do thread atual para todas as
        ///   pesquisas de recursos que usam essa classe de recurso de tipo de alta segurança.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Consulta uma cadeia de caracteres localizada semelhante a &lt;?xml version=&quot;1.0&quot; encoding=&quot;utf-8&quot; ?&gt;
        ///&lt;storyboard&gt;
        ///  &lt;viewpoint&gt;sulana&lt;/viewpoint&gt;
        ///  &lt;music&gt;never_look_back&lt;/music&gt;
        ///  &lt;background&gt;carmim&lt;/background&gt;
        ///  &lt;unset&gt;var1&lt;/unset&gt;
        ///  &lt;set&gt;var2&lt;/set&gt;
        ///  &lt;observe /&gt;
        ///  &lt;set&gt;++var3&lt;/set&gt;
        ///&lt;/storyboard&gt;.
        /// </summary>
        internal static string SimpleElements {
            get {
                return ResourceManager.GetString("SimpleElements", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Consulta uma cadeia de caracteres localizada semelhante a &lt;?xml version=&quot;1.0&quot; encoding=&quot;utf-8&quot; ?&gt;
        ///&lt;storyboard&gt;
        ///  &lt;background&gt;imagem&lt;/background&gt;
        ///  &lt;narration&gt;Primeira fala&lt;/narration&gt;
        ///  &lt;narration&gt;Primeira segunda fala&lt;/narration&gt;
        ///  &lt;narration&gt;Terceira fala&lt;/narration&gt;
        ///  &lt;music&gt;musica&lt;/music&gt;
        ///&lt;/storyboard&gt;.
        /// </summary>
        internal static string SimpleNarration {
            get {
                return ResourceManager.GetString("SimpleNarration", resourceCulture);
            }
        }
    }
}
