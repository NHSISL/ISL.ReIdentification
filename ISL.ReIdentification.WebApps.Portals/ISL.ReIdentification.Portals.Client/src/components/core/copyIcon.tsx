import { faCheck, faCopy } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { FunctionComponent, useState } from "react";

type CopyIconProps = {
    content: string;
    resetTime?: number;
    iconText?: string;
}

const CopyIcon : FunctionComponent<CopyIconProps> = (props) => {
    const {content, resetTime, iconText} = props
    const [copied, setCopied] = useState(false);

    const copyToClipboard = (content: string) => {
        navigator.clipboard.writeText(content)
        setCopied(true)
        if(resetTime) {
            setTimeout(() => setCopied(false), resetTime)
        }
    }

    return <span style={{cursor: 'pointer' }} onClick={() => copyToClipboard(content)}>{iconText ? <>{iconText}&nbsp;</> :'' }<FontAwesomeIcon icon={copied ? faCheck : faCopy}  /></span>
}

export default CopyIcon;