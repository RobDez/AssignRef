import React, { useState, useEffect } from "react";
import PropTypes from "prop-types";
import {
  Dropdown,
  DropdownToggle,
  DropdownItem,
  DropdownMenu,
} from "reactstrap";
import DefaultImage from "../../assets/images/avatar/anonymous_profile.png";
import { SlOptionsVertical } from "react-icons/sl";
import { FaUserEdit } from "react-icons/fa";
import * as Icon from "react-bootstrap-icons";
function MappingCertResults(props) {
  const aCertResult = props.certification;

  const [compData, setCompData] = useState({
    currentCert: {},
  });
  useEffect(() => {
    setCompData((prevState) => {
      let compD = { ...prevState };
      compD.currentCert = props.certification.certification;
      return compD;
    });
  }, []);

  useEffect(() => {
    aCertResult.isCompleted
      ? setIsEditing(false)
      : _logger("cert not completed");
  });
  const [showDropdown, setShowDropdown] = useState(false);
  const [isEditing, setIsEditing] = useState(false);
  const [CertificationStatus, setCertificationStatus] = useState(aCertResult);

  const handleCertificationChange = (event) => {
    const updatedData = event.target.value;
    const property = event.target.id;



    let updateCertResult = { ...aCertResult, [property]: updatedData };

    if (property === "isPhysicalCompleted") {
      updateCertResult.isPhysicalCompleted = updatedData;
    } else if (property === "isBackgroundCheckCompleted") {
      updateCertResult.isBackgroundCheckCompleted = updatedData;
    } else if (property === "isTestCompleted") {
      updateCertResult.isTestCompleted = updatedData;
    } else if (property === "isFitnessTestCompleted") {
      updateCertResult.isFitnessTestCompleted = updatedData;
    } else if (property === "isClinicAttended") {
      updateCertResult.isClinicAttended = updatedData;
    }

    props.onUpdate(updateCertResult);
    setCertificationStatus((prevState) => ({
      ...prevState,
      [property]: updatedData,
    }));
  };

  const toggleModal = () => {
    setShowDropdown(!isEditing);
  };

  const onLocalEdit = () => {
    setIsEditing(true);
    props.onEdit(aCertResult);
  };

  return (
    <tbody role="rowgroup">
      <tr role="row" className=" align-middle ">
        <td role="cell">
          <div>
            <span>
              <img
                src={aCertResult?.user?.avatarUrl || DefaultImage}
                alt="pic"
                className="rounded-circle avatar-md me-2"
              />
              <span className="fw-bold text-secondary px-1 ">
                {aCertResult.user?.firstName} {aCertResult.user?.lastName}
              </span>
            </span>
          </div>
        </td>
        {compData.currentCert.isPhysicalRequired ? (
          <td role="cell">
            <div className="my-2 text-center px-5">
              {isEditing ? (
                <select
                  value={CertificationStatus.isPhysicalCompleted}
                  id="isPhysicalCompleted"
                  name="isPhysicalCompleted"
                  onChange={handleCertificationChange}
                  onClick={onLocalEdit}
                  className="form-select form-select-sm"
                >
                  <option value={true}>Yes</option>
                  <option value={false}>No</option>
                </select>
              ) : (
                <span>{`${
                  aCertResult.isPhysicalCompleted ? "Yes" : "No"
                }`}</span>
              )}
            </div>
          </td>
        ) : (
          <td role="cell"></td>
        )}
        {compData.currentCert.isBackgroundCheckRequired ? (
          <td role="cell">
            <div className="my-2 text-center px-8">
              {isEditing ? (
                <select
                  value={CertificationStatus.isBackgroundCheckCompleted}
                  id="isBackgroundCheckCompleted"
                  name="isBackgroundCheckCompleted"
                  onChange={handleCertificationChange}
                  className="form-select form-select-sm"
                >
                  <option value={true}>Yes</option>
                  <option value={false}>No</option>
                </select>
              ) : (
                <span>{`${
                  aCertResult.isBackgroundCheckCompleted ? "Yes" : "No"
                }`}</span>
              )}
            </div>
          </td>
        ) : (
          <td role="cell"></td>
        )}
        {compData.currentCert.isTestRequired ? (
          <td role="cell">
            <div className="my-2 text-center px-4">
              {isEditing ? (
                <select
                  value={CertificationStatus.isTestCompleted}
                  id="isTestCompleted"
                  name="isTestCompleted"
                  onChange={handleCertificationChange}
                  className="form-select form-select-sm"
                >
                  <option value={true}>Yes</option>
                  <option value={false}>No</option>
                </select>
              ) : (
                <span>{`${aCertResult.isTestCompleted ? "Yes" : "No"}`}</span>
              )}
            </div>
          </td>
        ) : (
          <td role="cell"></td>
        )}
        {compData.currentCert.isFitnessTestRequired ? (
          <td role="cell">
            <div className="my-2 text-center px-4">
              {isEditing ? (
                <select
                  value={CertificationStatus.isFitnessTestCompleted}
                  id="isFitnessTestCompleted"
                  name="isFitnessTestCompleted"
                  onChange={handleCertificationChange}
                  className="form-select form-select-sm"
                >
                  <option value={true}>Yes</option>
                  <option value={false}>No</option>
                </select>
              ) : (
                <span>{`${
                  aCertResult.isFitnessTestCompleted ? "Yes" : "No"
                }`}</span>
              )}
            </div>
          </td>
        ) : (
          <td role="cell"></td>
        )}
        {compData.currentCert.isClinicRequired ? (
          <td role="cell">
            <div className="my-2 text-center px-4">
              {isEditing ? (
                <select
                  value={CertificationStatus.isClinicAttended}
                  id="isClinicAttended"
                  name="isClinicAttended"
                  onChange={handleCertificationChange}
                  className="form-select form-select-sm"
                >
                  <option value={true}>Yes</option>
                  <option value={false}>No</option>
                </select>
              ) : (
                <span>{`${aCertResult.isClinicAttended ? "Yes" : "No"}`}</span>
              )}
            </div>
          </td>
        ) : (
          <td role="cell"></td>
        )}
        <td role="cell">
          {aCertResult.isCompleted ? (
            <div className="text-success">
              <Icon.CheckCircle size={30} />
            </div>
          ) : (
            <div className="my-2 cursor-pointer">
              <Dropdown isOpen={showDropdown} toggle={toggleModal}>
                <DropdownToggle tag="span">
                  <SlOptionsVertical title="More Options" />
                </DropdownToggle>
                <DropdownMenu className="opacity-100">
                  <DropdownItem onClick={onLocalEdit}>
                    <FaUserEdit title="Edit" className="text-warning mx-3" />
                    Edit
                  </DropdownItem>
                </DropdownMenu>
              </Dropdown>
            </div>
          )}
        </td>
      </tr>
    </tbody>
  );
}

MappingCertResults.propTypes = {
  onUpdate: PropTypes.func.isRequired,

  certification: PropTypes.shape({
    certification: PropTypes.shape({
      isBackgroundCheckRequired: PropTypes.bool.isRequired,
      isClinicRequired: PropTypes.bool.isRequired,
      isFitnessTestRequired: PropTypes.bool.isRequired,
      isTestRequired: PropTypes.bool.isRequired,
    }),
    user: PropTypes.shape({
      firstName: PropTypes.string.isRequired,
      lastName: PropTypes.string.isRequired,
      avatarUrl: PropTypes.string,
    }),
    isPhysicalCompleted: PropTypes.bool.isRequired,
    isBackgroundCheckCompleted: PropTypes.bool.isRequired,
    isTestCompleted: PropTypes.bool.isRequired,
    isFitnessTestCompleted: PropTypes.bool.isRequired,
    isClinicAttended: PropTypes.bool.isRequired,
    isCompleted: PropTypes.bool.isRequired,
  }),
  onEdit: PropTypes.func.isRequired,
};

export default MappingCertResults;
