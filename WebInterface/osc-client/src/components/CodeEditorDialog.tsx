import React, { useState } from "react";
import { Dialog, Button, Classes } from "@blueprintjs/core";
import AceEditor from "react-ace";
import { DEFAULT_PROCESSOR } from "../definitions";

// Import a mode (language) and theme for the editor
import "ace-builds/src-noconflict/mode-javascript";
import "ace-builds/src-noconflict/theme-monokai";


interface CodeEditorDialogProps {
    title : string;
    isOpen: boolean;
    onClose: () => void;
    onSubmit: (code: string) => void;
}

export function CodeEditorDialog(props : CodeEditorDialogProps) {
    const { title, isOpen, onClose, onSubmit } = props;
 
    const [code, setCode] = useState(DEFAULT_PROCESSOR); // <- we will have to change this when loading states that are serialized

    const handleCodeChange = (_code : string) => {
        setCode(_code);
    };

    const handleSubmit = () => {
        onSubmit(code);
        onClose();
    }
    
    return (
        <div>
            {/* <Button onClick={handleOpen}>Open Editor</Button> */}
            <Dialog
                title={title}
                isOpen={isOpen}
                onClose={onClose}
            >
                <div className="bp3-dialog-body">
                    <AceEditor
                        mode="javascript"
                        theme="monokai"
                        onChange={handleCodeChange}
                        value={code}
                        // name="UNIQUE_ID_OF_DIV"
                        editorProps={{ $blockScrolling: true }}
                        setOptions={{
                            enableBasicAutocompletion: true,
                            enableLiveAutocompletion: true,
                            enableSnippets: true,
                            showLineNumbers: true,
                            tabSize: 2,
                        }}
                    />
                </div>
                <div className={Classes.DIALOG_FOOTER}>
                    <div className={Classes.DIALOG_FOOTER_ACTIONS}>
                    <Button onClick={onClose}>Close</Button>
                    <Button intent="primary" onClick={handleSubmit}>Submit</Button>
                    </div>
                </div>
            </Dialog>
        </div>
    );
}

export default CodeEditorDialog;