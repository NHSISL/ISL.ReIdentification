import { faCheck, faCopy } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { FunctionComponent, useState } from "react";

type CopyIconProps = {
    content: string;
    resetTime?: number
}

const CopyIcon : FunctionComponent<CopyIconProps> = (props) => {
    const {content, resetTime} = props
    const [copied, setCopied] = useState(false);

    const copyToClipboard = (content: string) => {
        navigator.clipboard.writeText(content)
        setCopied(true)
        if(resetTime) {
            setTimeout(() => setCopied(false), resetTime)
        }
    }

    return <FontAwesomeIcon icon={copied ? faCheck : faCopy} onClick={() => copyToClipboard(content)} />
}

export default CopyIcon;