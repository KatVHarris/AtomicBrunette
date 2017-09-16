using UnityEngine;
using System;
using System.IO;
using System.Text;
using Rosettastone.Speech;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;

public class UnityGetSpeechModelTask : GetSpeechModelTask {
	protected SpeechModelDescriptor _descriptor;
	protected string _configDir;
	protected Rosettastone.Speech.Logger _logger;
	protected SpeechModel _speechModel;
	
	public UnityGetSpeechModelTask ( string configDir, SpeechModelDescriptor descriptor, Rosettastone.Speech.Logger logger ) : 
		base(logger, "UnityGetSpeechModelTask") {
		_configDir = configDir;
		_descriptor = descriptor;
		_logger= logger;
		_logger.debug("UnityGetSpeechModelTask", "Created get model task " + getSWIGCPtrGetSpeechModelTask().Handle );
	}
	
	public override void customRun () {
		try {
			_logger.debug("GetSpeechModelTask", "customRun" );
			
			_speechModel = new SpeechModel( _descriptor ); 
			this.speechModel = _speechModel;
			speechModel.setConfigDirectory(_configDir);
			speechModel.setType(SpeechModel.ModelType.SPEECH_MODEL_TYPE_ON_DISK);
			extractModelAsset( _descriptor.language, _descriptor.voiceType );
			taskComplete();
		} catch {
			taskError("Error extracting model");
		}
	}

	void extractModelAsset(string languageISO, string voiceType) {
		string assetPath= "models/" + languageISO + ".zip";
		assetPath= AssetHelper.extractSingleAsset(assetPath);
		unzipFile(assetPath, _configDir );
	}
	
	void unzipFile(string zipfileName, string dest) {
		_logger.debug ("UnityGetSpeechModelTask", "unzipping " + zipfileName + " to " + dest);
		ZipFile zipFile= null;

		try {
			zipFile= new ZipFile(File.OpenRead(zipfileName));
			foreach (ZipEntry entry in zipFile) {
				if (entry.IsDirectory) continue;
				
				_logger.debug ("UnityGetSpeechModelTask", "\t " + entry.Name);
				string unzipPath= Path.Combine(dest, entry.Name);
				string unzipDir= Path.GetDirectoryName(unzipPath);
				Directory.CreateDirectory(unzipDir);

				byte[] buffer= new byte[4096];
				Stream zipStream= zipFile.GetInputStream(entry);
				using (FileStream writer = File.Create(unzipPath)) {
					while (true) {
						int bytesRead = zipStream.Read(buffer, 0, buffer.Length);
						if (bytesRead > 0) {
							writer.Write(buffer, 0, bytesRead);
						} else {
							writer.Flush();
							break;
						}
					}
				}
			}
		} finally {
			if (zipFile != null) {
				zipFile.Close();
			}
		}
	}
}
